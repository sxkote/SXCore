using SXCore.Common.Enums;
using SXCore.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SXCore.Common.Values
{
    public class ParamValue
    {
        protected string _name;
        protected string _value;
        protected ParamType _type;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ParamType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        protected ParamValue()
        {
        }

        public ParamValue(string name, string value)
        {
            _name = name;
            _value = value;
            _type = ParamType.String;
        }

        public ParamValue(string name, string value, ParamType type)
        {
            _name = name;
            _value = String.IsNullOrEmpty(value) ? "" : value.Trim();
            _type = type;
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

            var item = obj as ParamValue;
            if (item == null)
                return false;

            return this.Name.Equals(item.Name) && this.Value.Equals(item.Value);
        }

        public KeyValuePair<string, string> GetKeyValuePair()
        { return new KeyValuePair<string, string>(this.Name, this.Value); }
    }

    public class ParamValueCollection : List<ParamValue>
    {
        private string _prefix = "";

        public string Prefix
        {
            get { return _prefix; }
            private set { _prefix = String.IsNullOrEmpty(value) ? "" : value.Trim('.'); }
        }

        public ParamValueCollection(string prefix = "")
            : base()
        { this.Prefix = prefix ?? ""; }

        public ParamValueCollection(IEnumerable<ParamValue> collection, string prefix = "")
            : base(collection)
        { this.Prefix = prefix ?? ""; }

        public string this[string name]
        {
            get { return this.GetValue(name); }
            set
            {
                var item = this.Find(name);
                if (item != null)
                    this.Remove(item);

                this.Add(new ParamValue(name, value));
            }
        }

        public void Add(string name, string value, ParamType type = ParamType.String)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Имя параметра не может быть пусто");

            this.Add(new ParamValue((this.Prefix + "." + name).Trim('.'), value, type));
        }

        public void AddNested(IEnumerable<ParamValue> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
                this.Add(value.Name, value.Value, value.Type);
        }

        public ParamValue Find(string name)
        {
            return this.FirstOrDefault(v => v.Name.Equals(name, CommonService.StringComparison));
        }

        public string GetValue(string name)
        {
            var item = this.Find(name);

            return item == null ? null : item.Value;
        }

        public string Replace(string text)
        {
            if (String.IsNullOrEmpty(text))
                return "";

            var result = Regex.Replace(text, @"#(?<paramname>[\w\-\.\d]+)(?<!\.)", match =>
            {
                var paramname = match.Groups["paramname"].Value; //.Trim('.');
                var paramvalue = this.GetValue(paramname);
                return paramvalue == null ? "" : paramvalue.ToString();
            });

            return result;
        }

        public List<KeyValuePair<string, string>> GetKeyValuePairs()
        {
            return this.Select(i => i.GetKeyValuePair()).ToList();
        }
    }
}
