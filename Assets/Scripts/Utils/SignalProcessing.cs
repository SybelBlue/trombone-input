using System.Collections.Generic;

namespace SignalProcessing
{
    public struct Filter
    {
        // the amount of entries the filter remembers and stores before returning a
        // fitlered value
        private static int queue_length = 3;
        // small cyclic containers for fast reading and writing
        private readonly uint[] input;
        private readonly uint?[] output;

        private bool jumpedFromZero;
        // current index into input and output
        private int currentIndex;
        // current input
        private uint currentInput
        {
            get => input[currentIndex];
            set => input[currentIndex] = value;
        }
        // current output
        private uint? currentOutput
        {
            get => output[currentIndex];
            set => output[currentIndex] = value;
        }
        // output one entry ago
        private uint? prevOutput
        {
            get => output[(currentIndex + queue_length - 1) % queue_length];
            set => output[(currentIndex + queue_length - 1) % queue_length] = value;
        }
        // output two entries ago
        private uint? pprevOutput => output[(currentIndex + queue_length - 2) % queue_length];
        // input one entry ago
        private uint prevInput => input[(currentIndex + queue_length - 1) % queue_length];
        // input two entries ago
        private uint pprevInput => input[(currentIndex + queue_length - 2) % queue_length];

        private readonly uint epsilon, deadzone;

        public Filter(uint epsilon, uint deadzone)
        {
            this.input = new uint[queue_length];
            this.output = new uint?[queue_length];
            this.jumpedFromZero = false;

            this.epsilon = epsilon;
            this.deadzone = deadzone;
            this.currentIndex = -1;
        }

        public uint? Push(uint rawIn)
        {
            currentIndex = (currentIndex + 1) % queue_length;
            currentInput = rawIn;

            // if in deadzone -> 0
            // if just jumping from 0 -> 0
            // if did jump from 0 -> can't jump again, raw
            // if dropping to 0 -> prev = pprev
            // if near prev and pprev inputs -> prev + pprev / 2
            // else raw
            bool inDeadzone = rawIn < deadzone;
            bool jumpingFromZero = prevOutput == 0 && rawIn >= deadzone + epsilon;
            bool steepDrop = prevOutput >= 2.5f * epsilon + currentInput;
            bool inNeighborhood = currentNeighbor(prevInput) && currentNeighbor(pprevInput);

            if (inDeadzone)
            {
                currentOutput = 0;
            }
            else if (jumpingFromZero)
            {
                currentOutput = jumpedFromZero ? rawIn : 0;
                jumpedFromZero = !jumpedFromZero;
            }
            else if (steepDrop)
            {
                currentOutput = 0;
                prevOutput = pprevOutput;
            }
            else if (inNeighborhood)
            {
                currentOutput = (pprevInput + prevInput) / 2;
            }
            else
            {
                currentOutput = rawIn;
            }

            return pprevOutput;
        }

        // is within radius epsilon of currentInput
        private bool currentNeighbor(uint value)
            => currentInput - epsilon <= value && value <= currentInput + epsilon;

        // get the partially fitlered values currently in storage
        // warning: these values may change before they are returned by using Push
        public void DumpPartials(ref List<uint> list)
        {
            list.OptionalAdd(pprevOutput);
            list.OptionalAdd(prevOutput);
            list.OptionalAdd(currentOutput);
        }
    }

    public static class Utils
    {
        // returns a filtered list of the provided data, constructing a new filter from the optional arguments
        public static List<uint> BatchFilter(IEnumerable<uint> data, uint epsilon = 2, uint deadzone = 8)
        {
            Filter filter = new Filter(epsilon, deadzone);
            return BatchFilter(ref filter, data);
        }

        // returns a filtered list of the data, modifying the provided filter in the process
        public static List<uint> BatchFilter(ref Filter filter, IEnumerable<uint> data)
        {
            var list = new List<uint>();

            foreach (var item in data)
            {
                list.OptionalAdd(filter.Push(item));
            }
            filter.DumpPartials(ref list);

            return list;
        }
    }
}