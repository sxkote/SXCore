using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class Period
    {
        private DateTime? _begin;
        private DateTime? _end;

        public DateTime? Begin { get { return _begin; } }
        public DateTime? End { get { return _end; } }

        public Period(DateTime? begin, DateTime? end)
        {
            _begin = begin;
            _end = end;
        }
    }
}
