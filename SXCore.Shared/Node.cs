using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace SXCore.Shared
{
    public class SXNode
    {
        #region Variables
        protected SXNode parent = null;

        protected SXAttributeList attributes = new SXAttributeList();

        protected List<SXNode> nodes = new List<SXNode>();

        protected string prefix = "";

        protected string name = "";
        protected string value = "";
        #endregion

        #region Properties
        public SXNode Parent
        { get { return this.parent; } }

        public SXAttributeList Attributes
        { get { return this.attributes; } }

        public List<SXNode> Nodes
        { get { return this.nodes; } }

        public string Prefix
        { get { return this.prefix; } }

        public string Name
        { get { return this.name; } }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string TagName
        { get { return ((this.Prefix.Trim() != "") ? (this.Prefix + ":") : "") + this.Name.Trim(); } }

        protected int Level
        {
            get
            {
                int result = 0;
                SXNode current_parent = this.Parent;
                while (current_parent != null)
                {
                    current_parent = current_parent.Parent;
                    result++;
                }
                return result;
            }
        }

        public SXNode Root
        {
            get
            {
                SXNode result = this;
                while (result.Parent != null)
                    result = result.Parent;
                return result;
            }
        }
        #endregion

        #region Initializing
        public SXNode() { }

        public SXNode(Stream stream)
        {
            XmlReader reader = XmlReader.Create(stream);

            while (reader.Read())
                if (reader.IsStartElement())
                {
                    this.Initialize(reader, null);
                    return;
                }
        }

        public SXNode(string xml)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            while (reader.Read())
                if (reader.IsStartElement())
                {
                    this.Initialize(reader, null);
                    return;
                }
        }

        public SXNode(SXNode parent, string name, string value)
        {
            this.Initialize(parent);

            this.name = name;
            this.value = value;
        }

        protected SXNode(XmlReader reader, SXNode parent)
        { this.Initialize(reader, parent); }

        protected void Initialize(SXNode parent_node)
        { this.parent = parent_node; }

        protected bool Initialize(XmlReader reader, SXNode parent_node)
        {
            if (!reader.IsStartElement())
                return false;

            this.parent = parent_node;

            this.prefix = reader.Prefix;
            this.name = reader.LocalName;

            bool empty_element = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
                this.attributes.Add(new SXAttribute(reader));

            if (!empty_element)
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                        this.nodes.Add(new SXNode(reader, this));
                    else if (reader.NodeType == XmlNodeType.Text)
                        this.value = reader.Value;
                    else if (reader.NodeType == XmlNodeType.EndElement)
                        break;
                }

            return true;
        }

        public override string ToString()
        {
            #region define space
            string space = "";
            //if (new_lines)
                for (int i = 0; i < this.Level; i++)
                    space += '\t';
            #endregion

            string tag_name = this.TagName;
            string tag_attr = this.Attributes.ToString();

            var builder = new StringBuilder();
            
            builder.Append(String.Format("{0}<{1}{2}>", space, tag_name, tag_attr));

            #region Content
            if (this.Nodes != null && this.Nodes.Count > 0)
                this.Nodes.ForEach(n => builder.Append(String.Format("{0}{1}{0}", SXLocal.NewLine, n)));
            else
                builder.Append(SXNode.XmlReplacesSave(this.Value));
            #endregion

            builder.Append(String.Format("{0}</{1}>", ((this.Nodes.Count > 0) ? space : ""), tag_name));

            return builder.ToString();
        }
        #endregion

        #region Functions
        public string GetAttribute(string name)
        { return this.Attributes.GetValue(name);}

        public SXNode GetNode(string name)
        {
            if (name == null) return null;

            string find_name = name.Trim().ToLower();
            foreach (SXNode n in this.Nodes)
                if (n.Name.Trim().ToLower() == find_name)
                    return n;
            return null;
        }

        public SXNode GetNodePath(string path)
        {
            if (path == null) return null;

            string[] names = path.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries );
            if (names == null || names.Length <= 0)
                return null;

            SXNode result = this;
            foreach (string n in names)
            {
                result = result.GetNode(n);
                if (result == null) 
                    return null;
            }

            return result;
        }

        public SXNode AddNode(string name, string value)
        {
            SXNode node = new SXNode(this, name, value);
            this.Nodes.Add(node);
            return node;
        }
        #endregion

        #region Statics
        public static string XmlReplacesSave(string save_string)
        { return ((save_string == null) ? "&null;" : save_string.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;")); }

        public static string XmlReplacesLoad(string save_string)
        { return ((save_string == "&null;") ? null : save_string.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&amp;", "&")); }
        #endregion
    }
}
