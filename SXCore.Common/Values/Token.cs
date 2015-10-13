using SXCore.Common.Enums;
using SXCore.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class Token<S, P, M> : ISubscriber<S, P, M>
    {
        protected ValuesCollection _values;

        public S SubscriptionID { get; private set; }
        public P PersonID { get; private set; }
        public M ManagerID { get; private set; }

        public string Login { get; private set; }
        public string Key { get; private set; }
        public DateTimeOffset? Expire { get; private set; }

        public Gender Gender { get; set; }
        public PersonName Name { get; set; }
        public string Avatar { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string[] Roles { get; set; }
        public string[] Pemissions { get; set; }

        public ValuesCollection ValuesCollection
        {
            get
            {
                if (_values == null)
                    _values = new ValuesCollection();
                return _values;
            }
            protected set
            {
                _values = value ?? new ValuesCollection();
            }
        }

        public Token(S subscriptionID, P personID, M managerID, string login, string key, DateTimeOffset? expire = null)
        {
            SubscriptionID = subscriptionID;
            PersonID = personID;
            ManagerID = managerID;
            Login = login;
            Key = key;
            Expire = expire;
        }

        public Token(Token<S, P, M> token)
        {
            if (token == null)
                return;

            SubscriptionID = token.SubscriptionID;
            PersonID = token.PersonID;
            ManagerID = token.ManagerID;

            Login = token.Login;
            Key = token.Key;
            Expire = token.Expire;

            Gender = token.Gender;
            Name = token.Name;
            Avatar = token.Avatar;

            Email = token.Email;
            Phone = token.Phone;

            Roles = token.Roles;
            Pemissions = token.Pemissions;

            _values = token.ValuesCollection;
        }

        public override string ToString()
        {
            return this.Key;
        }

        public string GetBasicAuthValue()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Login + ":" + this.Key));
        }
    }
}
