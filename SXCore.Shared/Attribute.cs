using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SXCore.Shared
{
    public class SXAttribute
    {
        #region Variables
        protected string uri = "";
        protected string prefix = "";
        protected string name = "";
        protected string value = "";
        #endregion

        #region Properties
        public string Uri
        { get { return this.uri; } }

        public string Prefix
        { get { return this.prefix; } }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #endregion

        #region Initializing
        public SXAttribute(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Attribute)
                return;

            this.uri = reader.NamespaceURI;
            this.prefix = reader.Prefix;
            this.name = reader.LocalName;
            this.value = reader.Value;
        }

        public SXAttribute(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public SXAttribute(string name, string value, string uri, string prefix)
        {
            this.name = name;
            this.value = value;
            this.uri = uri;
            this.prefix = prefix;
        }

        public SXAttribute(SXAttribute item)
        {
            this.uri = item.Uri;
            this.prefix = item.Prefix;
            this.name = item.Name;
            this.value = item.Value;
        }

        public override string ToString()
        { return (this.name.Trim() == "") ? "" : (((this.Prefix.Trim() != "") ? (this.Prefix + ":") : "") + this.name.Trim() + "=\"" + SXNode.XmlReplacesSave(this.value.Trim()) + "\""); }
        #endregion

        #region Statics
        public static SXAttribute GetAttribute(string attribute_string)
        {
            attribute_string = attribute_string.Trim();
            if (attribute_string == "")
                return null;

            int equal_pos = attribute_string.IndexOf('=');
            if (equal_pos <= 0 || equal_pos >= attribute_string.Length - 2)
                return null;

            string value = attribute_string.Substring(equal_pos + 1).Trim();
            if ((value[0] == '"' && value[value.Length - 1] == '"') || (value[0] == '\'' && value[value.Length - 1] == '\''))
                value = value.Substring(1, value.Length - 2).Trim();
            else return null;

            string title = attribute_string.Substring(0, equal_pos).Trim();
            if (title == "")
                return null;

            return new SXAttribute(title, value);
        }

        public static List<SXAttribute> CreateAttributeList(string attributes_string)
        {
            List<SXAttribute> result = new List<SXAttribute>();
            attributes_string = attributes_string.Trim();
            if (attributes_string == "")
                return result;

            attributes_string = attributes_string + ' ';
            bool quotes_balans = true;        //current char is out of the quotes
            bool value_started = false;       //the value was started
            for (int i = 0; i < attributes_string.Length; i++)
            {
                if (attributes_string[i] == '"' || attributes_string[i] == '\'')
                {
                    value_started = true;
                    quotes_balans = !quotes_balans;
                }

                if (value_started && quotes_balans && attributes_string[i] == ' ')
                {
                    SXAttribute current_attribute = SXAttribute.GetAttribute(attributes_string.Substring(0, i).Trim());
                    if (current_attribute != null)
                        result.Add(current_attribute);
                    result.AddRange(SXAttribute.CreateAttributeList(attributes_string.Substring(i).Trim()));
                    return result;
                }
            }

            return result;
        }

        
        #endregion
    }

    public class SXAttributeList : List<SXAttribute>
    {
        #region Properties
        public SXAttribute this[string name]
        { get { return this.Find(name); } }
        #endregion

        #region Constructors
        public SXAttributeList():base() { }
        public SXAttributeList(IEnumerable<SXAttribute> collection) : base(collection) { }
        #endregion

        #region Functions
        public override string ToString()
        { return this.ToString(true); }

        public string ToString(bool emptyValues)
        {
            string result = "";
            for (int i = 0; i < this.Count; i++)
                if (this[i].Value.Trim() != "" || emptyValues)
                    result += " " + this[i].ToString();
            return result;
        }

        public SXAttribute Find(string name)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Name.Trim().ToLower() == name.Trim().ToLower())
                    return this[i];
            return null;
        }

        public string GetValue(string name)
        {
            SXAttribute find = this.Find(name);
            return ((find == null) ? "" : find.Value);
        }

        public void Add(string name, string value)
        { this.Add(new SXAttribute(name, value)); }

        public void SetValue(string name, string value)
        {
            SXAttribute attr = this.Find(name);
            if (attr == null)
                this.Add(name, value);
            else
                attr.Value = value;
        }
        #endregion
    }
}
