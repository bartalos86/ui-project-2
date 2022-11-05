using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace genetic_algorithm
{
    public class VirtualMachine
    {
        public List<Memory> Memory { get; set; }
        public List<Step> Steps { get; private set; }
        public double Fitness { get; private set; }

        private Func<List<Step>,bool> swarmBoundControl;

        public VirtualMachine(Func<List<Step>, bool> swarmBoundsControl)
        {
            this.swarmBoundControl = swarmBoundsControl;
            Memory = new List<Memory>();
            InitializeMachine();
        }

        private void InitializeMachine()
        {
            for (int i = 0; i < 64; i++)
            {
                var binaryValue = GenerateBinaryValue();
                var binaryAddress = GenerateAddress(i);

                var memory = new Memory(binaryAddress, binaryValue);
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
                var targetAddressIndex = Convert.ToInt32(targetAddress,2);
                //Console.WriteLine(instuction + ": " + targetAddressIndex);

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
                    continue;
                }else if(instuction == Instruction.PRINT)
                {
                    Steps = GetSteps();

                    if (!IsInBounds())
                        break;
   
                }
            }

            Fitness = fitnessFunction.Invoke(this);

        }

        public List<Step> GetSteps()
        {
            //Console.WriteLine("----------------------------------");
            List<Step> steps = new List<Step>();
            foreach(var memory in Memory)
            {
                steps.Add(memory.GetStep());
            }

            return steps;
        }

        private bool IsInBounds()
        {
            return swarmBoundControl.Invoke(GetSteps());
        }


        private static string GenerateAddress(int number)
        {
            var binaryAddress = new StringBuilder(Convert.ToString(number, 2));
            Random random = new Random();

            while (binaryAddress.Length < 6)
            {
                binaryAddress.Insert(0, "0");
            }

            return binaryAddress.ToString();

        }

        private static string GenerateBinaryValue(int length = 8)
        {
            var value = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                if (random.Next(2) == 1)
                {
                    value.Append("1");
                }
                else
                {
                    value.Append("0");
                }
            }

            return value.ToString();
        }
    }
}
