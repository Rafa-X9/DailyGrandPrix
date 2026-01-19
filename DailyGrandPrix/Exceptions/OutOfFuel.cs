using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyGrandPrix.Exceptions
{
    internal class OutOfFuel : ApplicationException
    {
        public OutOfFuel(string message) : base(message)
        {
        }
    }
}
