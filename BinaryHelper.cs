using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace genetic_algorithm
{
    public static class BinaryHelper
    {
        public static bool IsOneAtPosition(this uint number, int position)
        {
            return (number & ((long)Math.Pow(2, 8 - position - 1))) != 0;
        }
    }
}
