using System;

namespace SXCore.Common.Values
{
    public class Period
    {
        public DateTimeOffset? Begin { get; private set; }
        public DateTimeOffset? End { get; private set; }

        public Period(DateTimeOffset? begin, DateTimeOffset? end)
        {
            this.Begin = begin;
            this.End = end;
        }
    }
}
