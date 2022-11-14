using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace genetic_algorithm
{
    public class VirtualMachine : IComparable<VirtualMachine>
    {
        public List<Memory> Memory { get; set; }
        public List<Step> Steps { get; private set; }
        public double Fitness { get; private set; }

        private Func<List<Step>, (bool, int)> swarmBoundControl;
        private int customSize = 64;
        private int customBits;

        public VirtualMachine(Func<List<Step>, (bool,int)> swarmBoundsControl, int customSize = 64)
        {
            this.swarmBoundControl = swarmBoundsControl;
            Memory = new List<Memory>();
            InitializeMachine(customSize);
        }

        private void InitializeMachine(int customSize)
        {
            this.customSize = customSize;
            this.customBits = (int) Math.Ceiling(Math.Log2(customSize)) + 2;
            int nearestSize = (int) Math.Pow(2, customBits-2);
            

            for (uint i = 0; i < nearestSize; i++)
            {
                var binaryValue = GenerateBinaryValue(customBits);
                var memory = new Memory(i, (uint)binaryValue, customBits);
                Memory.Add(memory);
            }
        }

        public void PrintOutMemory()
        {
            foreach(var memory in Memory)
            {
                Console.WriteLine(memory.ToString());
            }
        }

        public void DoGeneration(Func<VirtualMachine, double> fitnessFunction)
        {
            var instructionCounter = 0;
            for (int i = 0; i < Memory.Count; i++)
            {
                var memory = Memory[i];
                var instuction = memory.GetInstruction();
                var targetAddress = memory.GetValue();
                var targetAddressIndex = (int)targetAddress;
                if (instructionCounter >= 500)
                    break;

                instructionCounter++;

                if (instuction == Instruction.INCREMET)
                {
                   Memory[targetAddressIndex].IncrementValue();
                }else if(instuction == Instruction.DECREMENT)
                {
                    Memory[targetAddressIndex].DecrementValue();
                }else if(instuction == Instruction.JUMP)
                {
                    i = targetAddressIndex -1;
                }else if(instuction == Instruction.PRINT)
                {
                    if(Steps == null)
                    Steps ??= new List<Step>();
                    Steps.Add(Memory[targetAddressIndex].GetStep());

                    if (!IsInBounds().Item1)
                    {
                        Steps.RemoveAt(Steps.Count - 1);
                        break;
                    }

                }
            }

            Fitness = fitnessFunction.Invoke(this);

        }

        private (bool, int) IsInBounds()
        {
            return swarmBoundControl.Invoke(Steps);
        }

        private static int GenerateBinaryValue(int length = 8)
        {
            Random random = new Random();
            return random.Next((int)Math.Pow(2,length));
        }

        public int CompareTo(VirtualMachine? other)
        {
            if (other == null)
                return 1;

            return this.Fitness.CompareTo(other.Fitness);
        }
    }
}
