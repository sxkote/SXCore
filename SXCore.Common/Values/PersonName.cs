using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class PersonName
    {
        public string First { get; private set; }
        public string Last { get; private set; }

        private PersonName()
        {
            this.First = "";
            this.Last = "";
        }

        public PersonName(string first, string last)
        {
            this.First = String.IsNullOrEmpty(first) ? "" : first;
            this.Last = String.IsNullOrEmpty(last) ? "" : last;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", this.First, this.Last);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object name)
        {
            return name != null && name is PersonName && this == (PersonName)name;
        }

        public static bool operator ==(PersonName name1, PersonName name2)
        {
            if ((object)name1 == null && (object)name2 == null)
            {
                return true;
            }

            if ((object)name1 == null || (object)name2 == null)
            {
                return false;
            }

            if (!name1.Last.Equals(name2.Last) || !name1.First.Equals(name2.First))
                return false;

            return true;
        }

        public static bool operator !=(PersonName name1, PersonName name2)
        {
            return !(name1 == name2);
        }

        public static implicit operator PersonName(PersonFullName name)
        { return new PersonName(name.First, name.Last); }
    }

    public class PersonFullName
    {
        public string First { get; private set; }
        public string Last { get; private set; }
        public string Second { get; private set; }

        private PersonFullName()
        {
            this.First = "";
            this.Last = "";
            this.Second = "";
        }

        public PersonFullName(string first, string last, string second = "")
        {
            this.First = String.IsNullOrEmpty(first) ? "" : first;
            this.Last = String.IsNullOrEmpty(last) ? "" : last;
            this.Second = String.IsNullOrEmpty(second) ? "" : second;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", this.First, this.Second, this.Last);
        }

        public override bool Equals(object name)
        {
            return name != null && name is PersonFullName && this == (PersonFullName)name;
        }

        public static bool operator ==(PersonFullName name1, PersonFullName name2)
        {
            if ((object)name1 == null && (object)name2 == null)
            {
                return true;
            }

            if ((object)name1 == null || (object)name2 == null)
            {
                return false;
            }

            if (!name1.Last.Equals(name2.Last) || !name1.First.Equals(name2.First) || !name1.Second.Equals(name2.Second))
                return false;

            return true;
        }

        public static bool operator !=(PersonFullName name1, PersonFullName name2)
        {
            return !(name1 == name2);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }
}
