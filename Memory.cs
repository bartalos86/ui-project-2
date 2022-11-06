
namespace genetic_algorithm
{
    [Serializable]
    public class Memory
    {
        public string Address { get; set; }
        public string Value { get; set; }

        private int customSizeBits = 8;

        public Memory(string address, string value, int customSizeBits = 8)
        {
            Address = address;
            Value = value;
            this.customSizeBits = customSizeBits;
        }

        public Instruction GetInstruction()
        {
            return (Instruction) Convert.ToInt32(Value.Substring(0, 2), 2);
        }

        public string GetValue()
        {
            return Value.Substring(2, Value.Length - 2);
        }

        public void IncrementValue()
        {
            var value = Value;
            var intValue = Convert.ToInt32(value, 2) +1;

            if(intValue > 255)
            {
                value = "0000000";

                //while (value.Length < customSizeBits)
                //{
                //    value = value.Insert(0, "0");
                //}
            }
            else
            {
                value = Convert.ToString(intValue, 2);
                while (value.Length < customSizeBits)
                {
                   value = value.Insert(0, "0");
                }
            }

            Value = value;
        }

        public void DecrementValue()
        {
            var value = GetValue();
            var intValue = Convert.ToInt32(value, 2) - 1;

            if (intValue <= 0)
            {
                value = "1111111";
                //while (value.Length < customSizeBits)
                //{
                //    value = value.Insert(0, "1");
                //}
            }
            else
            {
                value = Convert.ToString(intValue, 2);
                while (value.Length < customSizeBits)
                {
                    value = value.Insert(0, "0");
                }
            }

            Value = value;
        }

        public override string ToString()
        {
            return "Address: " + Address + " ("+ Convert.ToInt32(Address,2) +") Value: " + Value + " GetValue: " + GetValue() ;
        }

        public Step GetStep()
        {
            int numberOfOnes = 0;
            foreach(char character in Value)
            {
                if (character == '1')
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
