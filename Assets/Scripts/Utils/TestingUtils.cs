using StaticUtils = Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace Testing
{
    public struct Trial
    {
        public readonly TrialItem[] items;
        public readonly int trialNumber;
        public readonly int Length;

        public Trial(params TrialItem[] items)
        {
            this.items = items;
            this.Length = items.Length;
            foreach (var item in items)
            {
                switch (item)
                {
                    case Command c when c.type == Command.CommandType.SetTrialNumber:
                        trialNumber = c.trialNumber.Value;
                        return;
                }
            }
            throw new ArgumentException("TrialItems do not contain an Identifying TrialNumber");
        }
    }
    public static class Utils
    {
        public const char COMMAND_PREFIX = '!', COMMENT_PREFIX = '#', CHALLENGE_SEPERATOR = ':';

        public static Trial ReadTrialItems(TextAsset trialFile, bool logComments = true)
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

            return new Trial(items.ToArray());
        }
    }

    public abstract class TrialItem
    {
        protected TrialItem()
        { }

        // returns wether or not this sets TrialController in a blocking state
        public abstract bool Apply(TestingController controller);
    }

    public class Command : TrialItem
    {
        public readonly CommandType type;
        public readonly int? trialNumber;

        public Command(CommandType command, int? trialNumber = null) : base()
        {
            this.type = command;
            this.trialNumber = trialNumber;
        }

        public Command(string command, string num) : this(StringIntoType(command), StringIntoTrialNumber(num))
        { }

        public override bool Apply(TestingController controller)
        {
            switch (type)
            {
                case CommandType.RandomizeLayoutOrder:
                    controller.RandomizeLayouts();
                    return false;
                case CommandType.AdvanceLayout:
                    controller.AdvanceLayout();
                    return false;
                case CommandType.SetTrialNumber:
                    return false;
            }

            throw new ArgumentException(type.ToString() + " not recognized");
        }

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

            throw new ArgumentException(s);
        }

        [Serializable]
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

        public override bool Apply(TestingController controller)
        {
            controller.currentPrompt = prompt;
            controller.currentChallengeType = type;
            return true;
        }

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

            throw new ArgumentException(type);
        }
    }

    [Serializable]
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