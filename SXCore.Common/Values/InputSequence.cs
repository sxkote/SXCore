using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class InputSequence
    {
        static public readonly char DefaultSeparator = ';';
        static public readonly char[] DefaultSeparators = { ';', ',' };

        protected readonly List<string> _values;

        public List<string> Values { get { return _values; } }

        public int Count { get { return _values == null ? 0 : _values.Count; } }

        public InputSequence()
        {
            _values = new List<string>();
        }

        public InputSequence(params string[] values)
        {
            _values = values == null ? new List<string>() : values.Select(v => v.Trim()).ToList();
        }

        public override string ToString()
        {
            return String.Join(InputSequence.DefaultSeparator.ToString(), this.Values);
        }

        public void ForEach(Action<string> action)
        {
            this.Values.ForEach(action);
        }

        public IEnumerable<T> Select<T>(Func<string, T> projection)
        {
            return this.Values.Select(v => projection(v));
        }

        static public implicit operator InputSequence(string input)
        {
            return InputSequence.Create(input);
        }

        static public implicit operator InputSequence(string[] input)
        {
            return InputSequence.Create(input);
        }

        static public implicit operator string[] (InputSequence sequence)
        {
            if (sequence == null)
                return null;

            return sequence.Values.ToArray();
        }

        static public InputSequence Create(string input, char[] separators)
        {
            if (String.IsNullOrEmpty(input))
                return null;

            return new InputSequence(input.Split(separators, StringSplitOptions.RemoveEmptyEntries));
        }

        static public InputSequence Create(string input)
        {
            return InputSequence.Create(input, InputSequence.DefaultSeparators);
        }

        static public InputSequence Create(IEnumerable<string> values)
        {
            if (values == null)
                return null;

            return new InputSequence(values.ToArray());
        }
    }
}
