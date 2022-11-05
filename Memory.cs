using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace genetic_algorithm
{
    public class Memory
    {
        public string Address { get; set; }
        public string Value { get; set; }

        public Memory(string address, string value)
        {
            Address = address;
            Value = value;
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
                value = "00000000";
            }
            else
            {
                value = Convert.ToString(intValue, 2);
                while (value.Length < 8)
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
                value = "11111111";
            }
            else
            {
                value = Convert.ToString(intValue, 2);
                while (value.Length < 8)
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

            if (numberOfOnes <= 2)
                return Step.H;
            else if (numberOfOnes <= 4)
                return Step.D;
            else if (numberOfOnes <= 6)
                return Step.P;
            else
                return Step.L;
        }
    }
}
