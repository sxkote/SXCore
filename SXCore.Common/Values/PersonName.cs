using System;

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
            return $"{this.First} {this.Last}";
        }

        public string ToString(string format)
        {
            if (String.IsNullOrEmpty(format))
                return this.ToString();

            return format
                .Replace("first", this.First).Replace("1", this.First)
                .Replace("last", this.Last).Replace("3", this.Last)
                .Trim();
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
            if (ReferenceEquals(name1, null) || ReferenceEquals(name2, null))
                return false;

            if (ReferenceEquals(name1, name2))
                return true;

            return name1.Last.Equals(name2.Last) && name1.First.Equals(name2.First);
        }

        public static bool operator !=(PersonName name1, PersonName name2)
        {
            return !(name1 == name2);
        }

        public static implicit operator PersonName(PersonFullName name)
        { return new PersonName(name.First, name.Last); }

        public PersonName Copy()
        {
            return new PersonName()
            {
                First = this.First,
                Last = this.Last
            };
        }
    }

    public class PersonFullName
    {
        public string First { get; private set; }
        public string Last { get; private set; }
        public string Second { get; private set; }

        private PersonFullName() { }

        public PersonFullName(string first, string last, string second = "")
        {
            this.First = String.IsNullOrEmpty(first) ? "" : first;
            this.Last = String.IsNullOrEmpty(last) ? "" : last;
            this.Second = String.IsNullOrEmpty(second) ? "" : second;
        }

        public override string ToString()
        {
            return $"{this.First} {this.Second} {this.Last}";
        }

        public string ToString(string format)
        {
            if (String.IsNullOrEmpty(format))
                return this.ToString();

            return format
                .Replace("first", this.First).Replace("1", this.First)
                .Replace("second", this.Second).Replace("2", this.Second)
                .Replace("last", this.Last).Replace("3", this.Last)
                .Trim();
        }

        public string ToShortString()
        {
            return $"{this.First} {this.Last}".Trim();
        }

        public override bool Equals(object name)
        {
            return name != null && name is PersonFullName && this == (PersonFullName)name;
        }

        public static bool operator ==(PersonFullName name1, PersonFullName name2)
        {
            if (ReferenceEquals(name1, null) || ReferenceEquals(name2, null))
                return false;

            if (ReferenceEquals(name1, name2))
                return true;

            return name1.Last.Equals(name2.Last) && name1.First.Equals(name2.First) && name1.Second.Equals(name2.Second);
        }

        public static bool operator !=(PersonFullName name1, PersonFullName name2)
        {
            return !(name1 == name2);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public PersonFullName Copy()
        {
            return new PersonFullName()
            {
                First = this.First,
                Last = this.Last,
                Second = this.Second
            };
        }

        static public implicit operator PersonName(PersonFullName name)
        {
            if (name == null)
                return null;

            return new PersonName(name.First, name.Last);
        }
    }

    public class PersonTotalName
    {
        public string First { get; private set; }
        public string Last { get; private set; }
        public string Second { get; private set; }
        public string Maiden { get; private set; }

        private PersonTotalName()
        {
            this.First = "";
            this.Last = "";
            this.Second = "";
            this.Maiden = "";
        }

        public PersonTotalName(string first, string last, string second = "", string maiden = "")
        {
            this.First = String.IsNullOrEmpty(first) ? "" : first;
            this.Last = String.IsNullOrEmpty(last) ? "" : last;
            this.Second = String.IsNullOrEmpty(second) ? "" : second;
            this.Maiden = String.IsNullOrEmpty(maiden) ? "" : maiden;
        }

        public override string ToString()
        {
            var maidenValue = String.IsNullOrEmpty(this.Maiden) ? "" : $"({this.Maiden})";
            return $"{this.First} {this.Second} {this.Last} {maidenValue}".Trim();
        }

        public string ToString(string format)
        {
            if (String.IsNullOrEmpty(format))
                return this.ToString();

            return format
                .Replace("first", this.First).Replace("1", this.First)
                .Replace("second", this.Second).Replace("2", this.Second)
                .Replace("last", this.Last).Replace("3", this.Last)
                .Replace("maiden", this.Maiden).Replace("4", this.Maiden)
                .Trim();
        }

        public string ToShortString()
        {
            return $"{this.First} {this.Last}".Trim();
        }

        public string ToFullString()
        {
            return $"{this.First} {this.Second} {this.Last}".Trim();
        }

        public override bool Equals(object name)
        {
            return name != null && name is PersonTotalName && this == (PersonTotalName)name;
        }

        public static bool operator ==(PersonTotalName name1, PersonTotalName name2)
        {
            if ((object)name1 == null && (object)name2 == null)
            {
                return true;
            }

            if ((object)name1 == null || (object)name2 == null)
            {
                return false;
            }

            if (!name1.Last.Equals(name2.Last) || !name1.First.Equals(name2.First) || !name1.Second.Equals(name2.Second) || !name1.Maiden.Equals(name2.Maiden))
                return false;

            return true;
        }

        public static bool operator !=(PersonTotalName name1, PersonTotalName name2)
        {
            return !(name1 == name2);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        static public implicit operator PersonFullName(PersonTotalName name)
        {
            if (name == null)
                return null;

            return new PersonFullName(name.First, name.Last, name.Second);
        }

        static public implicit operator PersonName(PersonTotalName name)
        {
            if (name == null)
                return null;

            return new PersonName(name.First, name.Last);
        }
    }
}
