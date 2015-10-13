using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class ValueItem
    {
        private string _key;
        private string _value;

        public string Key
        {
            get { return _key; }
            protected set { _key = value; }
        }

        public string Value
        {
            get { return _value; }
            protected set { _value = value; }
        }

        private ValueItem()
        {
            _key = "";
            _value = "";
        }

        public ValueItem(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;

            if (Object.ReferenceEquals(obj, this))
                return true;

            var item = obj as ValueItem;
            if (item == null)
                return false;

            return this.Key.Equals(item.Key) && this.Value.Equals(item.Value);
        }
    }

    public class ValuesCollection : List<ValueItem>
    {
        public ValuesCollection() : base() { }
        public ValuesCollection(IEnumerable<ValueItem> collection) : base(collection) { }

        public string this[string key]
        {
            get { return this.GetValue(key); }
            set
            {
                var item = this.Find(key);
                if (item != null)
                    this.Remove(item);

                this.Add(key, value);
            }
        }

        public void Add(string key, string value)
        {
            this.Add(new ValueItem(key, value));
        }

        public ValueItem Find(string key)
        {
            return this.FirstOrDefault(v => v.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        }

        //public void Add(params ValueItem[] items)
        //{
        //    if (items != null)
        //        foreach (var item in items)
        //            this.Add(item);
        //}

        public string GetValue(string key)
        {
            var item = this.Find(key);

            return item == null ? null : item.Value;
        }

        public string Replace(string text)
        {
            if (String.IsNullOrEmpty(text))
                return "";

            var result = Regex.Replace(text, @"#(?<paramname>[\w\-\.\d]+)", match =>
            {
                var value = this.GetValue(match.Groups["paramname"].Value);
                return value == null ? "" : value.ToString();
            });

            return result;
        }
    }
}
