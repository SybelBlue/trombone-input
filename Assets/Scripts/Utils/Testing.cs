using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using System;
using Controller;
using Utils.SystemExtensions;
using Utils.UnityExtensions;
using CustomInput;
using System.Linq;

namespace Testing
{
    public readonly struct Trial
    {
        public readonly TrialItem[] items;
        public readonly int trialNumber;
        public readonly int Length;

        public Trial(params TrialItem[] items)
        {
            this.items = items;
            Length = items.Length;

            foreach (var item in items)
            {
                switch (item)
                {
                    case SetTrialCommand c:
                        trialNumber = c.data;
                        return;
                }
            }
            throw new ArgumentException("TrialItems do not contain an Identifying TrialNumber");
        }
    }

    public static class Utils
    {
        public const char COMMAND_PREFIX = '!', COMMENT_PREFIX = '#', CHALLENGE_SEPERATOR = ':';

        public static List<Trial> ReadTrials(bool logComments = true)
        {
            List<Trial> trials = new List<Trial>();

            var dir = Path.Combine(Application.streamingAssetsPath, "Trials");
            var pattern = "*.txt";
            foreach (string file in Directory.EnumerateFiles(dir, pattern))
            {
                UsingStream(file, reader => trials.Add(ReadTrialItems(reader, logComments)));
            }
            Debug.Log($"Loaded {trials.Count} trials ({trials.Select(t => t.Length).Sum()} TrialItems)");
            return trials;
        }

        public static Trial ReadTrialItems(TextAsset asset, bool logComments = true)
        {
            using (StreamReader reader = new StreamReader(asset.IntoMemoryStream()))
            {
                return ReadTrialItems(reader, logComments);
            }
        }

        public static Trial ReadTrialItems(StreamReader sr, bool logComments = true)
        {
            List<TrialItem> items = new List<TrialItem>();

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith(COMMENT_PREFIX.ToString()))
                {
                    if (logComments)
                    {
                        Debug.Log(line.Substring(1));
                    }
                    continue;
                }

                if (line.StartsWith(COMMAND_PREFIX.ToString()))
                {
                    line = line.Substring(1);
                    string[] parts = line.Split(new char[] { ' ' });

                    items.Add(Command.fromString(parts[0], parts.Length > 1 ? parts[1] : null));
                    continue;
                }

                int index = line.IndexOf(CHALLENGE_SEPERATOR);

                if (index < 0 || line.Length <= index)
                {
                    Debug.LogError($"unkown line: {line}");
                    continue;
                }

                items.Add(new Challenge(line.Substring(0, index), line.Substring(index + 1)));
            }

            return new Trial(items.ToArray());
        }

        public static (string directory, string name) WriteTrialResults(List<ITrialResult> t, bool locally = false)
        {
            string directory = locally ? "Assets/Results" : Path.Combine(Application.persistentDataPath, "TrialResults");
            string name = UniqueYamlName("trial");
            string path = $"{directory}/{name}";

            UsingStream(path, writer => WriteTrialResults(t, writer), append: false);

            if (!locally)
            {
                Debug.Log(directory);
            }

            return (directory, name);
        }

        public static string UniqueYamlName(string stub)
            => $"{stub}-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.yaml";

        public static void UsingStream(string path, Action<StreamReader> ReaderCallback)
        {
            Touch(path);

            using (StreamReader reader = new StreamReader(path, true))
            {
                ReaderCallback(reader);
            }
        }

        public static void UsingStream(string path, Action<StreamWriter> WriterCallback, bool append = true)
        {
            Touch(path);

            using (StreamWriter writer = new StreamWriter(path, append))
            {
                WriterCallback(writer);
            }
        }

        private static void Touch(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        public static void WriteTrialResults(List<ITrialResult> t, StreamWriter writer)
        {
            if (t.Count == 0) return;
            writer.WriteLine($"meta:");
            writer.WriteLine($"  timestamp: \"{DateTime.Now}\"");
            // https://stackoverflow.com/questions/17632584/how-to-get-the-unix-timestamp-in-c-sharp
            writer.WriteLine($"  unixseconds: {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
            writer.WriteLine($"  platform: {Application.platform}");
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);
            writer.WriteLine($"  xrdevices: {(devices.Count > 0 ? "" : "[]")}");
            foreach (var device in devices)
            {
                writer.WriteLine($"    {device.name}: {(device.characteristics == 0 ? "None" : "")}");
                uint max = (uint)InputDeviceCharacteristics.Simulated6DOF;
                for (int i = 1; i <= max; i <<= 1)
                {
                    var flag = (InputDeviceCharacteristics)i;
                    if ((device.characteristics & flag) == flag)
                    {
                        writer.WriteLine($"      - {Enum.GetName(typeof(InputDeviceCharacteristics), flag)}");
                    }
                }
            }

            writer.WriteLine("trial:");
            foreach (var item in t)
            {
                item.Write(writer);
            }
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static float BlindAccuracy(string prompt, string output)
        {
            float count = 0;
            for (int i = 0; i < Mathf.Min(prompt.Length, output.Length); i++)
            {
                count += prompt[i] == output[i] ? 1 : 0;
            }

            return count / Mathf.Max(prompt.Length, output.Length);
        }

        public static float PerfectAccuracy(string prompt, List<(string key, float time)> keypresses)
            => keypresses.Count / (float)prompt.Length;
    }

    public class ResultBuilder
    {
        private List<ITrialResult> results;

        private ChallengeResult lastChallenge;

        public ResultBuilder()
            => results = new List<ITrialResult>();

        public void Push(TrialItem item, string currentOutput, string currentLayoutName)
        {
            if (lastChallenge != null)
            {
                EndLastChallenge(currentOutput);
            }

            switch (item)
            {
                case Challenge challenge:
                    results.Add(lastChallenge = new ChallengeResult(challenge, currentLayoutName));
                    return;
                case Command command:
                    results.Add(new CommandResult(command));
                    return;
            }

            throw new ArgumentException($"{item.GetType()} not recognized");
        }

        public void EndLastChallenge(string output)
        {
            if (lastChallenge == null) return;
            lastChallenge.SetEndNow();
            lastChallenge.output = output;
            lastChallenge = null;
        }

        public List<ITrialResult> Finish(string output)
        {
            EndLastChallenge(output);
            return results;
        }

        public void AddKeypress(char c)
            => lastChallenge.AddKeypress($"{c}");

        public void AddKeypress(string c)
            => lastChallenge.AddKeypress(c);
    }

    public interface ITrialResult
    {
        void Write(StreamWriter writer);
    }

    public abstract class TrialItemResult<T> : ITrialResult
        where T : TrialItem
    {
        public readonly T source;

        public TrialItemResult(T source)
        {
            this.source = source;
        }

        public abstract void Write(StreamWriter writer);
    }

    public class CommandResult : TrialItemResult<Command>
    {
        public CommandResult(Command source) : base(source)
        { }

        public override void Write(StreamWriter writer)
        {
            writer.WriteLine($"  - command: {source.key}");
            if (source is CommandWithData)
            {
                writer.WriteLine($"    data: {(source as CommandWithData).data}");
            }
        }
    }

    public class ChallengeResult : TrialItemResult<Challenge>
    {
        public readonly string layoutName;
        public float start;
        public float? end;

        public string output;

        public List<(string key, float time)> keypresses;

        public ChallengeResult(Challenge source, string layoutName, float? start = null) : base(source)
        {
            this.layoutName = layoutName;
            this.start = start ?? Time.time;

            output = "";
            keypresses = new List<(string key, float time)>(source.prompt.Length * 2);
        }

        private int _indent = 0;
        private void Indent() => _indent++;
        private void Unindent() => _indent--;
        private string indent => "  ".Repeat(_indent);
        private const string bullet = "- ";

        private float accuracy =>
            source.type == Challenge.Type.Perfect ?
                Testing.Utils.PerfectAccuracy(source.prompt, keypresses) :
                Testing.Utils.BlindAccuracy(source.prompt, output);

        public override void Write(StreamWriter writer)
        {
            Indent();
            writer.WriteLine($"{indent}{bullet}challenge:");
            Indent();
            Indent();
            writer.WriteLine($"{indent}type: {source.type.ToString()}");
            writer.WriteLine($"{indent}layout: {layoutName}");
            writer.WriteLine($"{indent}prompt: \"{source.prompt}\"");
            writer.WriteLine($"{indent}output: \"{output}\"");
            writer.WriteLine($"{indent}accuracy: {accuracy}");
            writer.WriteLine($"{indent}time:");
            Indent();
            writer.WriteLine($"{indent}start: {start}");
            writer.WriteLine($"{indent}stop: {end}");
            writer.WriteLine($"{indent}duration: {(end.HasValue ? end - start : null)}");
            Unindent();
            writer.WriteLine($"{indent}keypresses:");
            Indent();
            foreach ((string key, float time) in keypresses)
            {
                writer.WriteLine($"{indent}{time}: \"{(key == "\b" ? "\\b" : key)}\"");
            }
            _indent = 0;
        }

        public void SetEndNow()
            => end = Time.time;

        public void AddKeypress(string c)
            => keypresses.Add((c, Time.time));
    }

    public abstract class TrialItem
    {
        protected TrialItem()
        { }

        // returns wether or not this sets TrialController in a blocking state
        public abstract bool Apply(Proctor proctor);
    }

    public abstract class Command : TrialItem
    {
        public const string RANDOMIZE_KEY = "randomize-layouts", ADVANCE_KEY = "next-layout", SET_TRIAL_KEY = "trial-number", SET_LAYOUT_KEY = "set-layout";

        public abstract string key { get; }

        public Command() : base()
        { }

        public static Command fromString(string key, string maybeValue)
        {
            switch (key)
            {
                case RANDOMIZE_KEY:
                    return new RandomizeLayoutsCommand();
                case ADVANCE_KEY:
                    return new AdvanceLayoutCommand();
                case SET_TRIAL_KEY:
                    return new SetTrialCommand(maybeValue);
                case SET_LAYOUT_KEY:
                    return new SetLayoutCommand(maybeValue);
            }
            throw new ArgumentException($"Invalid key: {key} (with data {maybeValue})");
        }
    }

    public class RandomizeLayoutsCommand : Command
    {
        public override string key => RANDOMIZE_KEY;

        public RandomizeLayoutsCommand() : base()
        { }

        public override bool Apply(Proctor proctor)
        {
            proctor.RandomizeLayouts();
            return false;
        }
    }

    public class AdvanceLayoutCommand : Command
    {
        public override string key => ADVANCE_KEY;

        public AdvanceLayoutCommand() : base()
        { }

        public override bool Apply(Proctor proctor)
        {
            proctor.AdvanceLayout();
            return false;
        }
    }

    public abstract class CommandWithData : Command
    {
        public readonly int data;

        public CommandWithData(int data) : base()
        {
            this.data = data;
        }

        public static int fromString(string s)
        {
            if (!int.TryParse(s, out int o))
            {
                throw new ArgumentException("invalid int literal");
            }
            return o;
        }
    }

    public class SetTrialCommand : CommandWithData
    {
        public override string key => SET_TRIAL_KEY;

        public SetTrialCommand(string s) : base(fromString(s))
        { }

        public override bool Apply(Proctor _)
            => false;
    }

    public class SetLayoutCommand : CommandWithData
    {
        public override string key => SET_LAYOUT_KEY;

        public SetLayoutCommand(string s) : base(fromString(s))
        { }

        public override bool Apply(Proctor proctor)
        {
            proctor.SetLayout((LayoutOption)data);
            return false;
        }
    }

    public class Challenge : TrialItem
    {
        public const string BLIND_KEY = "blind", PERFECT_KEY = "perfect", PRACTICE_KEY = "practice";
        public readonly string prompt;
        public readonly Type type;
        public Challenge(Type type, string prompt) : base()
        {
            this.prompt = prompt;
            this.type = type;
        }

        public Challenge(string type, string prompt) : this(StringIntoType(type), prompt)
        { }

        public override bool Apply(Proctor proctor)
        {
            proctor.currentPrompt = prompt;
            proctor.currentChallengeType = type;
            return true;
        }

        public static Type StringIntoType(string type)
        {
            switch (type)
            {
                case BLIND_KEY:
                    return Type.Blind;
                case PERFECT_KEY:
                    return Type.Perfect;
                case PRACTICE_KEY:
                    return Type.Practice;
            }

            throw new ArgumentException(type);
        }

        public static string TypeIntoString(Type type)
            => new string[] { BLIND_KEY, PERFECT_KEY, PRACTICE_KEY }[(int)type];

        [Serializable]
        public enum Type
        {
            // no backspaces, no viewing output
            Blind,
            // must be 100% correct to advance
            Perfect,
            // may be skipped at any time
            Practice,
        }
    }
}