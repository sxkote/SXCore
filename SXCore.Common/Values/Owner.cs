using System;

namespace SXCore.Common.Values
{
    public class Owner
    {
        public const char Delimeter = '-';

        public int Type { get; private set; }
        public long ID { get; private set; }

        public Owner()
        {
            this.ID = 0;
            this.Type = 0;
        }

        public Owner(int type, long id)
        {
            this.Type = type;
            this.ID = id;
        }

        public Owner(string uri)
        {
            this.ID = 0;
            this.Type = 0;

            if (String.IsNullOrEmpty(uri))
                return;

            string[] parts = uri.Split(Owner.Delimeter);
            if (parts == null || parts.Length != 2)
                return;

            this.Type = Convert.ToInt32(parts[0]);
            this.ID = Convert.ToInt64(parts[1]);
        }

        public override string ToString()
        {
            return String.Format("{0}{1}{2}", this.Type, Owner.Delimeter, this.ID);
        }

        public static implicit operator Owner(string str)
        { return new Owner(str); }

        public static implicit operator string(Owner owner)
        { return owner.ToString(); }

    }
}
