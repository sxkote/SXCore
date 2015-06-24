using System;
using System.Collections.Generic;
using System.Text;

namespace SXCore.Shared
{
    public class SXState : List<SXStateItem>
    {
        #region Properties
        public bool Correct
        {
            get
            {
                foreach (SXStateItem i in this)
                    if (!i.Correct)
                        return false;
                return true;
            }
        }
        #endregion

        #region Constructors
        public SXState() { }
        public SXState(ICollection<SXStateItem> collecion) : base(collecion) { }
        #endregion

        #region Functions
        public void AddError(string name, string comment)
        { this.Add(SXStateItem.Error(name, comment)); }
        public void AddError(string comment)
        { this.Add(SXStateItem.Error(comment)); }
        #endregion
    }

    public class SXStateItem
    {
        public enum SXStateItemType { None, Error, Message };

        #region Variables
        protected long id = 0;
        protected string name = "";
        protected string comment = "";
        #endregion

        #region Properties
        public long ID { get { return this.id; } set { this.id = value; } }

        public string Name { get { return this.name; } set { this.name = value; } }

        public string Comment { get { return this.comment; } set { this.comment = value; } }

        public bool Correct { get { return (this.ID >= 0); } }

        public SXStateItemType Type { get { return ((this.ID > 0) ? SXStateItemType.Message : ((this.ID < 0) ? SXStateItemType.Error : SXStateItemType.None)); } }
        #endregion

        #region Constructors
        public SXStateItem(long id, string name, string comment)
        {
            this.ID = id;
            this.Name = name;
            this.Comment = comment;
        }

        public SXStateItem(long id, string comment)
            : this(id, "", comment) { }
        #endregion

        #region Statics
        static public SXStateItem Error(long id, string name, string comment)
        { return new SXStateItem(((id == 0) ? -13 : ((id > 0) ? -id : id)), name, comment); }
        static public SXStateItem Error(string name, string comment)
        { return SXStateItem.Error(-13, name, comment); }
        static public SXStateItem Error(string comment)
        { return SXStateItem.Error(-13, "", comment); }
        #endregion
    }
}
