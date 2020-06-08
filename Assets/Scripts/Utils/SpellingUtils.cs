using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AutoComplete
{
    public class AutoComplete
    {
        public static readonly int _completion_count = 16;
        private static AutoComplete _instance;

        private AutoComplete()
            => _instance = this;

        public static AutoComplete Instance
            => _instance == null ? new AutoComplete() : _instance;

        private PruningRadixTrie trie;

        public bool dictionaryLoaded => trie != null;

        public void InitDictionary(TextAsset frequencyDict, char separator)
        {
            trie = new PruningRadixTrie();
            using (Stream corpusStream = Utils.StreamFromTextAsset(frequencyDict))
            {
                trie.ReadTermsFromStream(corpusStream, separator);
            }
            Debug.LogWarning($"Trie Loaded! ({trie.termCountLoaded} nodes)");
        }

        // TODO: figure out why I'm broken
        public List<string> Completions(string prefix)
            => trie?.GetTopkTermsForPrefix(prefix.ToLower(), _completion_count, out long termFreqCountPrefix).Select(t => t.term).ToList();

    }
}

namespace AutoCorrect
{
    using Loader = System.Func<Stream, int, int, char[], bool>;

    [Serializable]
    public enum DictionarySize
    {
        None,
        Dict82765,
        Dict243342,
    }

    public class AutoCorrect
    {
        private static AutoCorrect _instance;

        private AutoCorrect()
            => _instance = this;

        public static AutoCorrect Instance
            => _instance == null ? new AutoCorrect() : _instance;

        private const int INIT_CAPACITY = 82765;
        private const int MAX_EDIT_DISTANCE_DICT = 2;

        private readonly SymSpell symSpell = new SymSpell(INIT_CAPACITY, MAX_EDIT_DISTANCE_DICT);

        private TextAsset DictionaryTextAsset(DictionarySize size, params TextAsset[] dicts)
        {
            switch (size)
            {
                case DictionarySize.Dict82765:
                    return dicts[0];

                case DictionarySize.Dict243342:
                    return dicts[1];
            }

            throw new ArgumentException("Unknown size");
        }

        private static (int, int) DictionaryTermAndCountIndices(DictionarySize size)
        {
            switch (size)
            {
                case DictionarySize.Dict82765:
                    return (0, 1);

                case DictionarySize.Dict243342:
                    return (0, 2);
            }

            throw new ArgumentException("Unknown size");
        }

        private Loader DictionaryLoader(DictionarySize size)
        {
            switch (size)
            {
                case DictionarySize.Dict82765:
                    return symSpell.LoadDictionary;

                case DictionarySize.Dict243342:
                    return symSpell.LoadBigramDictionary;
            }

            throw new ArgumentException("Unknown size");
        }

        public bool dictionaryLoaded { get; protected set; }

        public void InitDictionary(DictionarySize size, params TextAsset[] dicts)
        {
            dictionaryLoaded = false;

            if (size == DictionarySize.None) return;

            // must load basic first or else error...
            if (size != DictionarySize.Dict82765)
            {
                InitDictionary(DictionarySize.Dict82765);
            }

            Loader loader = DictionaryLoader(size);

            var (termIndex, countIndex) = DictionaryTermAndCountIndices(size);

            using (Stream corpusStream = Utils.StreamFromTextAsset(DictionaryTextAsset(size, dicts)))
            {
                if (!loader(corpusStream, termIndex, countIndex, SymSpell.defaultSeparatorChars))
                {
                    throw new Exception("Could not load dictionary!");
                }
                else
                {
                    Debug.LogWarning($"Dictionary Loaded! ({symSpell.EntryCount} entries)");
                    dictionaryLoaded = true;
                }
            }
        }

        public static int? DictionaryValue(string word)
        {
            long value = -1;
            if (Instance.dictionaryLoaded)
            {
                Instance.symSpell.words.TryGetValue(word.ToLower(), out value);
            }

            // if dict loaded and key present
            if (value > 0) return int.MaxValue > value ? (int)value : int.MaxValue;

            return null;
        }

        public List<SymSpell.SuggestItem> Lookup(string inputTerm, SymSpell.Verbosity verbosity)
        {
            int maxEditDistanceLookup = Mathf.Min(inputTerm.Length, MAX_EDIT_DISTANCE_DICT);
            // Assert.IsTrue(maxEditDistanceLookup <= MAX_EDIT_DISTANCE_DICT);
            if (inputTerm.Length == 0) return new List<SymSpell.SuggestItem>();

            // use LookupCompound?
            return symSpell.LookupCompound(inputTerm, maxEditDistanceLookup).Union(symSpell.Lookup(inputTerm, verbosity, maxEditDistanceLookup).Take(5)).ToList();
            // return symSpell.Lookup(inputTerm, verbosity, maxEditDistanceLookup);
        }

        public List<string> Suggestions(string inputTerm, SymSpell.Verbosity verbosity)
            => Lookup(inputTerm, verbosity).Select(suggestion => suggestion.term).Where(s => !s.ToUpper().Equals(inputTerm)).ToList();
    }

    public static class Disambiguator
    {
        public static List<string> Disambiguated(List<string> keypresses)
        {
            return GenerateScores(keypresses).Select(t => t.Item1).ToList();
        }

        private static IEnumerable<(string, float)> GenerateScores(List<string> keypresses, int maximum = 5)
        {
            if (keypresses.Count == 0) return new List<(string, float)>();

            List<(char, (string, float))> intermediate = new List<(char, (string, float))>();
            string firstKey = keypresses[0];

            foreach (char c in firstKey)
            {
                var s = new string(new char[] { c });
                intermediate.Add((c, (s, firstKey.Length > 1 ? ReadLog10Odds(' ', c) : 0.0f)));
            }

            for (int i = 1; i < keypresses.Count; i++)
            {
                string key = keypresses[i];
                List<(char, (string, float))> next = new List<(char, (string, float))>();
                foreach (var (last, (word, score)) in intermediate)
                {
                    if (key.Length == 1)
                    {
                        next.Add((key[0], (word + key[0], score)));
                    }
                    else
                    {
                        foreach (char letter in key)
                        {
                            next.Add((letter, (word + letter, score + ReadLog10Odds(last, letter))));
                        }
                    }
                }
                intermediate = next;
            }

            return intermediate
                    .Select(t => t.Item2)
                    .OrderByDescending(t => AutoCorrect.DictionaryValue(t.Item1) ?? t.Item2)
                    .Take(maximum);
        }

        private static float ReadLog10Odds(char prev, char curr)
        {
            int? prevIndex = IndexOf(prev), currIndex = IndexOf(curr);

            return prevIndex.HasValue && currIndex.HasValue ? log10Odds[prevIndex.Value][currIndex.Value] : 0;
        }

        private static int? IndexOf(char c)
        {
            if (c == ' ') return 26;

            int x = (int)char.ToUpper(c);
            if ((int)'A' <= x && x <= (int)'Z')
            {
                return x - (int)'A';
            }

            return null;
        }

        // Auto-generated 
        private static readonly float[][] log10Odds = new float[][] {
            new float[] {   -3.06444917f,   -1.34919979f,   -1.21516227f,   -1.48849199f,   -1.68345804f,   -2.13915731f,   -1.53927729f,   -2.30992097f,   -1.65223576f,   -2.96856224f,   -1.98626255f,   -0.86290212f,   -1.41207009f,   -0.86846275f,   -2.88103975f,   -1.44724319f,   -2.93823498f,   -0.96919091f,   -1.24991434f,   -0.84715106f,   -1.73403540f,   -1.92754218f,   -2.18050954f,   -2.36174839f,   -2.05418248f,   -2.30249187f,   -1.25686109f    },
            new float[] {   -0.86700280f,   -1.57252262f,   -2.21470802f,   -2.15063419f,   -0.85838793f,   -2.64741013f,   -2.86625337f,   -2.53393102f,   -0.85579745f,   -2.39753266f,   -3.24947013f,   -0.71196585f,   -2.43102428f,   -2.58566454f,   -0.96272682f,   -2.49190541f,   -3.80577263f,   -1.02079892f,   -1.62250278f,   -2.11380752f,   -1.13878691f,   -2.65655351f,   -2.91367802f,   -4.20371264f,   -2.03052637f,   -3.80577263f,   -2.00095195f    },
            new float[] {   -0.81779080f,   -3.82291533f,   -1.80697273f,   -3.46036730f,   -1.01134033f,   -3.90588957f,   -4.03851513f,   -0.88749570f,   -1.12466345f,   -14.0f, -1.37006265f,   -1.47809607f,   -3.46863982f,   -2.82670832f,   -0.80160017f,   -3.56139388f,   -2.93422317f,   -1.26368546f,   -2.17138450f,   -1.17172162f,   -1.29522940f,   -4.88361317f,   -3.88361317f,   -5.18464317f,   -1.63331518f,   -3.53143065f,   -1.04823972f    },
            new float[] {   -1.11734458f,   -2.49992503f,   -2.75061200f,   -1.79325968f,   -0.70847354f,   -2.51851393f,   -2.04139268f,   -2.37347254f,   -0.76714953f,   -2.58546072f,   -3.39105022f,   -1.57423274f,   -2.26211741f,   -1.90029807f,   -1.13384135f,   -2.78899023f,   -3.97462681f,   -1.35232642f,   -1.61971842f,   -2.89244005f,   -1.51788131f,   -2.47977679f,   -2.36008111f,   -4.75277806f,   -1.80363211f,   -3.45174806f,   -0.56394797f    },
            new float[] {   -1.39843751f,   -2.09255501f,   -1.45579881f,   -1.08606506f,   -1.71167804f,   -1.99891232f,   -1.95163849f,   -2.41106400f,   -1.93563049f,   -2.97801903f,   -2.73372941f,   -1.28374630f,   -1.47105643f,   -0.99097603f,   -1.83725078f,   -1.64071107f,   -2.60676454f,   -0.75149724f,   -0.90426366f,   -1.31743620f,   -1.92346788f,   -2.04615555f,   -2.20151596f,   -1.86180386f,   -2.33441683f,   -2.92250170f,   -0.82015186f    },
            new float[] {   -1.04213488f,   -2.90351078f,   -3.00264225f,   -2.99164687f,   -0.88088886f,   -1.10770167f,   -3.21349562f,   -2.97045757f,   -0.74693690f,   -3.69061687f,   -3.51452561f,   -0.98049951f,   -2.94049434f,   -2.95025418f,   -0.84980531f,   -3.13130886f,   -14.0f, -1.14161361f,   -2.09127974f,   -1.50770315f,   -1.00175230f,   -4.29267686f,   -2.97045757f,   -4.11658560f,   -1.61919517f,   -3.81555561f,   -1.52477925f    },
            new float[] {   -1.02989865f,   -2.62045679f,   -3.38564306f,   -2.82370029f,   -0.84099149f,   -2.83433661f,   -1.59222248f,   -1.46014897f,   -0.98632572f,   -3.83794073f,   -3.37305394f,   -1.16430555f,   -1.99026527f,   -1.47292099f,   -1.23209719f,   -3.07827289f,   -4.91712198f,   -1.05546811f,   -1.69129599f,   -2.58874238f,   -1.33505862f,   -4.21815198f,   -2.59283952f,   -14.0f, -1.60241229f,   -3.83794073f,   -0.62724330f    },
            new float[] {   -0.81738650f,   -2.47838786f,   -2.90106825f,   -2.82564715f,   -0.69141459f,   -2.55222647f,   -3.07903551f,   -2.91630821f,   -0.79526452f,   -3.78943498f,   -3.17313455f,   -1.71388802f,   -2.00935781f,   -1.96901456f,   -0.79388753f,   -2.70784766f,   -3.88634499f,   -1.35486607f,   -2.04331996f,   -1.57107456f,   -1.51821313f,   -3.44701230f,   -2.35380293f,   -14.0f, -1.05319788f,   -3.71025373f,   -1.18233755f    },
            new float[] {   -1.18029901f,   -1.82623855f,   -0.93926500f,   -1.33668053f,   -1.36757644f,   -1.70030125f,   -1.64798277f,   -2.87230614f,   -2.73287687f,   -3.37498150f,   -2.07510457f,   -1.29230491f,   -1.57423319f,   -0.71102372f,   -1.15238325f,   -1.64860011f,   -2.79658543f,   -1.63300666f,   -0.93323877f,   -1.10015914f,   -2.14491982f,   -1.57277809f,   -3.32823810f,   -2.61076007f,   -3.53651404f,   -1.57210416f,   -1.96766787f    },
            new float[] {   -0.59762514f,   -3.43584436f,   -3.03790435f,   -2.78263185f,   -0.71362190f,   -14.0f, -3.43584436f,   -2.83378437f,   -1.12092430f,   -2.95872311f,   -3.13481437f,   -3.25975310f,   -3.13481437f,   -2.53275437f,   -0.72488124f,   -3.25975310f,   -14.0f, -2.69548167f,   -3.25975310f,   -3.25975310f,   -0.57163003f,   -3.73687436f,   -3.43584436f,   -14.0f, -2.89177632f,   -14.0f, -2.25975310f    },
            new float[] {   -1.05747858f,   -2.01674189f,   -2.66493361f,   -2.63596991f,   -0.53155990f,   -2.14732823f,   -2.87205910f,   -1.76090865f,   -0.77274302f,   -3.03042159f,   -2.30778767f,   -1.35135727f,   -2.04454623f,   -1.54243526f,   -1.41174605f,   -2.28223356f,   -4.42836160f,   -1.83507553f,   -1.18038833f,   -2.01674189f,   -1.74261986f,   -2.95124035f,   -1.95269041f,   -4.42836160f,   -1.60228680f,   -3.95124035f,   -0.87997218f    },
            new float[] {   -0.89100285f,   -2.41885644f,   -2.17992939f,   -1.92435727f,   -0.75107051f,   -2.31395412f,   -2.35889623f,   -2.87654549f,   -0.77268896f,   -3.98881526f,   -2.33512047f,   -1.01670503f,   -2.14402754f,   -2.25441552f,   -1.01239650f,   -2.12608773f,   -4.01109166f,   -3.02502743f,   -1.81767411f,   -1.73973926f,   -1.46202383f,   -2.25241876f,   -2.80697167f,   -4.28984526f,   -0.98457990f,   -3.58227508f,   -1.02684868f    },
            new float[] {   -0.72879673f,   -1.38536031f,   -3.01772739f,   -3.05826093f,   -0.74611578f,   -2.65843678f,   -3.45384704f,   -3.11895877f,   -0.76408235f,   -3.74329516f,   -3.46574626f,   -2.44112378f,   -1.48713466f,   -2.01686825f,   -0.92768246f,   -1.18200543f,   -4.24389751f,   -2.93214365f,   -1.75464234f,   -3.01344859f,   -1.41912105f,   -3.12442167f,   -3.05826093f,   -5.02204876f,   -1.57427075f,   -3.84595750f,   -1.00505718f    },
            new float[] {   -1.13012136f,   -2.24722255f,   -1.27115659f,   -1.22231420f,   -0.89173617f,   -1.86678248f,   -0.92746634f,   -2.25213336f,   -1.05235635f,   -2.68275695f,   -2.05761014f,   -2.14660502f,   -2.19468191f,   -1.80394032f,   -1.09028665f,   -1.99576475f,   -2.71111859f,   -2.08152974f,   -1.23873011f,   -0.93011989f,   -1.88178193f,   -2.13608195f,   -2.42729960f,   -3.36700370f,   -2.15763764f,   -2.67861684f,   -0.99154331f    },
            new float[] {   -1.83639454f,   -1.72091428f,   -1.36307978f,   -1.51342582f,   -1.94738711f,   -2.06927816f,   -1.37250033f,   -2.38871234f,   -1.63608106f,   -3.21037376f,   -2.14979775f,   -1.13834930f,   -1.16949081f,   -0.72798894f,   -1.52709426f,   -1.22807334f,   -2.88350956f,   -0.94116220f,   -1.20685763f,   -1.29031779f,   -1.11027014f,   -1.59479700f,   -1.75972740f,   -2.10076255f,   -2.37539959f,   -2.51932080f,   -1.84718651f    },
            new float[] {   -0.95721769f,   -2.74386525f,   -2.90333077f,   -3.08249126f,   -0.81961118f,   -2.81010644f,   -3.19229625f,   -0.83124146f,   -1.04311933f,   -3.75458911f,   -3.20436076f,   -1.18986389f,   -2.70537109f,   -2.32322535f,   -0.95614565f,   -1.50210017f,   -4.45355912f,   -0.88494677f,   -1.46779594f,   -1.39446925f,   -1.47423842f,   -4.15252912f,   -2.78377750f,   -5.05561911f,   -1.80325160f,   -5.05561911f,   -1.61834431f    },
            new float[] {   -2.44737955f,   -14.0f, -14.0f, -3.46856885f,   -2.99144759f,   -3.76959884f,   -3.76959884f,   -3.46856885f,   -2.51432634f,   -14.0f, -14.0f, -3.46856885f,   -3.76959884f,   -3.46856885f,   -3.16753885f,   -3.46856885f,   -3.16753885f,   -2.99144759f,   -3.07062884f,   -2.86650886f,   -0.00895022f,   -3.46856885f,   -3.46856885f,   -14.0f, -3.76959884f,   -14.0f, -2.29247759f    },
            new float[] {   -0.84443689f,   -1.90165262f,   -1.63220170f,   -1.63783604f,   -0.78887961f,   -2.20934566f,   -1.87850485f,   -2.07375275f,   -0.83947095f,   -3.12873816f,   -2.10630753f,   -1.94063224f,   -1.56472570f,   -1.75691355f,   -0.89842886f,   -1.84625348f,   -3.22387191f,   -1.63137037f,   -1.31721443f,   -1.46484680f,   -1.60792902f,   -2.15314315f,   -2.43598271f,   -4.31200800f,   -1.51134819f,   -3.31564229f,   -1.07379457f    },
            new float[] {   -1.43121282f,   -2.71448595f,   -1.41984176f,   -2.99019312f,   -1.07955621f,   -2.71269134f,   -2.97192182f,   -1.34933140f,   -1.14061057f,   -3.54717473f,   -2.13717021f,   -1.82393881f,   -1.53600353f,   -1.98799330f,   -1.45266916f,   -1.46641618f,   -2.38940734f,   -2.94972676f,   -1.07913960f,   -0.86539860f,   -1.37906751f,   -3.28448973f,   -2.24859839f,   -14.0f, -1.92029965f,   -4.09740309f,   -0.51953231f    },
            new float[] {   -1.03878418f,   -2.63263413f,   -2.11348465f,   -3.07116033f,   -0.71770928f,   -2.48951481f,   -2.93044711f,   -1.14532750f,   -0.66754089f,   -3.53734160f,   -3.52456731f,   -1.82296179f,   -2.41794782f,   -2.50789725f,   -1.03385582f,   -2.76681931f,   -4.06238641f,   -1.05442538f,   -1.60983335f,   -1.59804592f,   -1.50003406f,   -3.44960255f,   -2.30918649f,   -4.51831836f,   -1.49630262f,   -2.83193749f,   -1.04579176f    },
            new float[] {   -1.50296249f,   -1.39496456f,   -1.46310204f,   -1.55555013f,   -1.58084175f,   -2.13303718f,   -1.74508139f,   -3.09772324f,   -1.50065519f,   -3.21582255f,   -2.39381802f,   -0.96969342f,   -1.17546749f,   -0.67775176f,   -2.10104482f,   -1.41262158f,   -3.50612868f,   -0.96573816f,   -0.82143654f,   -1.14996286f,   -3.27381450f,   -2.46570002f,   -3.64179128f,   -2.51360749f,   -2.95159520f,   -2.67175451f,   -2.20935651f    },
            new float[] {   -0.81879613f,   -4.51949985f,   -3.67440181f,   -3.56525734f,   -0.27042384f,   -14.0f, -3.61640986f,   -4.51949985f,   -0.68948891f,   -14.0f, -3.74134860f,   -3.24074625f,   -4.04237859f,   -3.40555650f,   -1.11193900f,   -4.04237859f,   -14.0f, -2.49831055f,   -3.10452650f,   -3.61640986f,   -1.85768716f,   -2.87604717f,   -4.21846985f,   -14.0f, -2.39239505f,   -3.91743986f,   -2.37028074f    },
            new float[] {   -0.66455243f,   -1.96477744f,   -2.49912536f,   -1.95244370f,   -0.80693653f,   -2.21366314f,   -2.65141370f,   -1.18632842f,   -0.78892454f,   -3.65141370f,   -2.09511120f,   -1.56150859f,   -2.18901571f,   -1.36856510f,   -0.83903519f,   -2.33754648f,   -4.04935371f,   -1.48173927f,   -1.49365482f,   -2.13027562f,   -2.30116569f,   -4.04935371f,   -2.43656986f,   -14.0f, -2.14626373f,   -3.04935371f,   -1.37129081f    },
            new float[] {   -0.99971037f,   -2.40811581f,   -1.23343919f,   -2.76562716f,   -0.97362480f,   -2.40811581f,   -2.90695632f,   -1.74214607f,   -0.69048589f,   -14.0f, -3.71986967f,   -2.24274842f,   -2.50238573f,   -2.90695632f,   -1.14065789f,   -1.03323340f,   -2.90695632f,   -2.90695632f,   -2.01657829f,   -0.94901766f,   -1.56605481f,   -3.11780968f,   -2.45269794f,   -3.02089967f,   -1.14123046f,   -4.02089967f,   -1.00932922f    },
            new float[] {   -1.52770497f,   -2.17198804f,   -1.49265579f,   -1.67578695f,   -1.61976824f,   -2.49457321f,   -1.88916327f,   -2.52646235f,   -1.69001767f,   -3.84868165f,   -2.92960356f,   -1.26198185f,   -1.46865140f,   -1.48225869f,   -1.62105200f,   -1.27592618f,   -4.14971164f,   -1.53947747f,   -1.28579427f,   -1.50744503f,   -2.51624319f,   -3.26889805f,   -2.35592126f,   -2.54765165f,   -3.89443914f,   -2.50232867f,   -0.28310535f    },
            new float[] {   -0.78196437f,   -2.84677878f,   -2.96487809f,   -2.84677878f,   -0.37034743f,   -3.69187682f,   -3.21475556f,   -2.99290681f,   -0.80557414f,   -3.86796808f,   -3.02287004f,   -1.74575220f,   -2.99290681f,   -3.08981683f,   -0.90229611f,   -2.96487809f,   -3.56693808f,   -3.12760539f,   -2.99290681f,   -3.02287004f,   -2.05172678f,   -3.21475556f,   -2.91372557f,   -14.0f, -1.52357580f,   -1.41541501f,   -1.76759753f    },
            new float[] {   -1.16319948f,   -1.30319922f,   -1.06172405f,   -1.29571644f,   -1.41612719f,   -1.49303235f,   -1.52879069f,   -1.43024223f,   -1.44778275f,   -2.11500543f,   -1.97150684f,   -1.56823692f,   -1.27154893f,   -1.43934325f,   -1.46517027f,   -1.02599639f,   -2.31474348f,   -1.34345418f,   -0.97989519f,   -1.29372723f,   -1.21101797f,   -1.84167805f,   -1.75148614f,   -2.86331581f,   -2.51027754f,   -2.42624731f,   -14.0f  }
        };

    }
}