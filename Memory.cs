
namespace genetic_algorithm
{
    [Serializable]
    public class Memory
    {
        public uint Address { get; set; }
        public uint Value { get; set; }

        private int customSizeBits = 8;

        public Memory(uint address, uint value, int customSizeBits = 8)
        {
            Address = address;
            Value = value;
            this.customSizeBits = customSizeBits;
        }

        public Instruction GetInstruction()
        {
            switch((Value & 192))
            {
                case 192: //11
                    return Instruction.PRINT;
                case 128: //10
                    return Instruction.JUMP;
                case 64: //01
                    return Instruction.DECREMENT;
                default:
                    return Instruction.INCREMET;
            }
        }

        public uint GetValue()
        {
            return (Value & 63);
        }

        public void IncrementValue()
        {
            var value = Value;
            var intValue = value +1;

            if(intValue > 255)
            {
                value = 0;
            }

            Value = value;
        }

        public void DecrementValue()
        {
            var value = GetValue();
            var intValue = value - 1;

            if (intValue < 0)
            {
                value = 255;
            }

            Value = value;
        }

        public override string ToString()
        {
            return "Address: " + Address + " ("+ Convert.ToString(Address,2) +") Value: " + Convert.ToString(Value,2) + " GetValue: " + GetValue() ;
        }

        public Step GetStep()
        {
            int numberOfOnes = 0;
            for(int i = 0; i < 8; i++)
            {
               if(Value.IsOneAtPosition(i))
                numberOfOnes++;
            }

            int intervalAdjustment = (customSizeBits - 8); //12
            //1 0-4 -5
            //2 5-6 -2
            //4 7-8 -4

            if (numberOfOnes <= 2 + intervalAdjustment / 4)
                return Step.H;
            else if (numberOfOnes <= 4 + intervalAdjustment /2)
                return Step.D;
            else if (numberOfOnes <= 6 + intervalAdjustment)
                return Step.P;
            else
                return Step.L;
        }
    }
}
