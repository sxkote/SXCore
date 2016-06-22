using System;

namespace SXCore.Common.Values
{
    public class EventArgument<T> : EventArgs
    {
        public T Value { get; protected set; }

        public EventArgument(T value)
        {
            this.Value = value;
        }
    }
}
