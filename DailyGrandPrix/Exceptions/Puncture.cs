using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyGrandPrix.Exceptions
{
    internal class Puncture : ApplicationException
    {
        public Puncture(string message) : base(message)
        {
        }
    }
}
