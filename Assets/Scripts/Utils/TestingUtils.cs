using StaticUtils = Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Testing
{
    public static class Utils
    {
        public const char COMMAND_PREFIX = '!', COMMENT_PREFIX = '#', CHALLENGE_SEPERATOR = ':';

        public static List<TrialItem> ReadTrialItems(TextAsset trialFile, bool logComments = true)
        {
            List<TrialItem> items = new List<TrialItem>();

            using (StreamReader sr = new StreamReader(StaticUtils.StreamFromTextAsset(trialFile)))
            {
                string line;
                //process a single line at a time only for memory efficiency
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

                        items.Add(new Command(parts[0], parts.Length > 1 ? parts[1] : null));
                        continue;
                    }

                    int index = line.IndexOf(CHALLENGE_SEPERATOR);

                    if (index < 0)
                    {
                        Debug.LogError($"unkown line: {line}");
                        continue;
                    }

                    items.Add(new Challenge(line.Substring(0, index), line.Substring(index + 1)));
                }
            }
            return items;
        }
    }

    public abstract class TrialItem
    {
        protected TrialItem()
        { }
    }

    public class Command : TrialItem
    {
        public readonly CommandType command;
        public readonly int? trialNumber;

        public Command(CommandType command, int? trialNumber = null) : base()
        {
            this.command = command;
            this.trialNumber = trialNumber;
        }

        public Command(string command, string num) : this(StringIntoType(command), StringIntoTrialNumber(num))
        { }

        public static int? StringIntoTrialNumber(string x)
        {
            if (x == null) return null;

            int n;
            if (int.TryParse(x, out n))
            {
                return n;
            }

            return null;
        }

        public static CommandType StringIntoType(string s)
        {
            switch (s)
            {
                case "randomize-layouts":
                    return CommandType.RandomizeLayoutOrder;
                case "next-layout":
                    return CommandType.AdvanceLayout;
                case "trial-number":
                    return CommandType.SetTrialNumber;
            }

            throw new System.ArgumentException(s);
        }

        public enum CommandType
        {
            RandomizeLayoutOrder,
            AdvanceLayout,
            SetTrialNumber,
        }
    }

    public class Challenge : TrialItem
    {
        public readonly string prompt;
        public readonly ChallengeType type;
        public Challenge(ChallengeType type, string prompt) : base()
        {
            this.prompt = prompt;
            this.type = type;
        }

        public Challenge(string type, string prompt) : this(StringIntoType(type), prompt)
        { }

        public static ChallengeType StringIntoType(string type)
        {
            switch (type)
            {
                case "blind":
                    return ChallengeType.Blind;
                case "perfect":
                    return ChallengeType.Perfect;
                case "practice":
                    return ChallengeType.Practice;
            }

            throw new System.ArgumentException(type);
        }

        public enum ChallengeType
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