using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antiban
{
    public static class SortRules
    {
        public static TimeSpan IntervalPerMessageGlobal { get { return new(0, 0, 10); } }
        public static TimeSpan IntervalToNumber { get { return new(0, 1, 0); } }
        public static TimeSpan IntervalToMailing { get { return new(24, 0, 0); } }
    }
}
