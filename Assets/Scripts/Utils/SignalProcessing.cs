using System.Collections.Generic;

namespace SignalProcessing
{

    public class Filter : AbstractFilter
    {
        private bool midDrop;

        public Filter(uint epsilon, uint deadzone) : base(epsilon, deadzone)
        {
            this.midDrop = false;
        }

        public override uint? Push(uint rawIn)
        {
            AdvanceCurrent();
            current = new IOPair { input = rawIn, output = rawIn };

            // if in deadzone -> 0
            // if just jumping from 0 -> 0
            // if did jump from 0 -> can't jump again, raw
            // if dropping to 0 -> prev = pprev
            // if near prev and pprev inputs -> prev + pprev / 2
            // else raw
            bool inDeadzone = rawIn < deadzone;
            bool jumpingFromZero = prev.output == 0 && deadzone + epsilon <= rawIn;
            bool steepDrop = 2.5f * epsilon + current.input <= prev.output;
            bool steepJump = 1.5f * epsilon + prev.input <= current.input;
            bool inNeighborhood = currentNeighbor(prev.input) && currentNeighbor(pprev.input);
            midDrop = midDrop && !inDeadzone && !inNeighborhood;

            if (inDeadzone || midDrop)
            {
                currentOutput = 0;
            }
            else if (jumpingFromZero)
            {
                currentOutput = jumpedFromZero ? rawIn : 0;
                jumpedFromZero = !jumpedFromZero;
            }
            else if (steepJump)
            {
                currentOutput = rawIn + (3 * (current.input - prev.input)) / 4;
            }
            else if (steepDrop)
            {
                currentOutput = 0;
                prevOutput = pprev.output;
                midDrop = true;
            }
            else if (inNeighborhood)
            {
                currentOutput = (pprev.output + prev.output) / 2;
            }
            // else leave currentOuput as default (rawIn)

            return pprev.output;
        }
    }

    public class OldFilter : AbstractFilter
    {
        public OldFilter(uint epsilon, uint deadzone) : base(epsilon, deadzone)
        { }

        public override uint? Push(uint rawIn)
        {
            AdvanceCurrent();
            current = new IOPair { input = rawIn, output = null };

            // if in deadzone -> 0
            // if just jumping from 0 -> 0
            // if did jump from 0 -> can't jump again, raw
            // if dropping to 0 -> prev = pprev
            // if near prev and pprev inputs -> prev + pprev / 2
            // else raw
            bool inDeadzone = rawIn < deadzone;
            bool jumpingFromZero = prev.output == 0 && rawIn >= deadzone + epsilon;
            bool steepDrop = prev.output >= 2.5f * epsilon + current.input;
            bool inNeighborhood = currentNeighbor(prev.input) && currentNeighbor(pprev.input);

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
                prevOutput = pprev.output;
            }
            else if (inNeighborhood)
            {
                currentOutput = (pprev.input + prev.input) / 2;
            }
            else
            {
                currentOutput = rawIn;
            }

            return pprev.output;
        }
    }

    public abstract class AbstractFilter
    {
        // the amount of entries the filter remembers and stores before returning a
        // fitlered value
        protected static int queue_length = 3;
        // small cyclic containers for fast reading and writing
        protected readonly IOPair[] data;

        protected bool jumpedFromZero;
        // current index into input and output
        private int _curr;
        // current entry
        protected IOPair current
        {
            get => data[_curr];
            set => data[_curr] = value;
        }

        protected uint? currentOutput
        {
            set => data[_curr].output = value;
        }

        protected uint? prevOutput
        {
            set => data[(_curr + queue_length - 1) % queue_length].output = value;
        }

        // pair one entry ago
        protected IOPair prev
        {
            get => data[(_curr + queue_length - 1) % queue_length];
            set => data[(_curr + queue_length - 1) % queue_length] = value;
        }
        // pair two entries ago
        protected IOPair pprev => data[(_curr + queue_length - 2) % queue_length];

        protected readonly uint epsilon, deadzone;

        protected AbstractFilter(uint epsilon, uint deadzone)
        {
            this.data = new IOPair[queue_length];
            this.jumpedFromZero = false;

            this.epsilon = epsilon;
            this.deadzone = deadzone;
            this._curr = -1;
        }

        // is within radius epsilon of currentInput
        protected bool currentNeighbor(uint value)
            => current.input - epsilon <= value && value <= current.input + epsilon;

        protected void AdvanceCurrent()
            => _curr = (_curr + 1) % queue_length;


        // get the partially fitlered values currently in storage
        // warning: these values may change before they are returned by using Push
        public void DumpPartials(ref List<uint> list)
        {
            list.OptionalAdd(pprev.output);
            list.OptionalAdd(prev.output);
            list.OptionalAdd(current.output);
        }

        public abstract uint? Push(uint value);

        protected struct IOPair
        {
            public uint input;
            public uint? output;
        }
    }

    public static class Utils
    {
        // returns a filtered list of the provided data, constructing a new filter from the optional arguments
        public static List<uint> BatchFilter(IEnumerable<uint> data, uint epsilon = 2, uint deadzone = 8)
        {
            AbstractFilter filter = new OldFilter(epsilon, deadzone);
            return BatchFilter(ref filter, data);
        }

        // returns a filtered list of the data, modifying the provided filter in the process
        public static List<uint> BatchFilter(ref AbstractFilter filter, IEnumerable<uint> data)
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