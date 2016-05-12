using SXCore.Common.Contracts;
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
        public const string User = "User";
        public const string Supervisor = "Supervisor";
        public const string Administrator = "Administrator";


        protected ParamValueCollection _values;

        public S SubscriptionID { get; protected set; }
        public P PersonID { get; protected set; }
        public M ManagerID { get; protected set; }

        public string Login { get; protected set; }
        public string Key { get; protected set; }
        public DateTimeOffset? Expire { get; protected set; }

        public PersonName Name { get; set; }
        public string Avatar { get; set; }

        public string[] Roles { get; set; }
        public string[] Pemissions { get; set; }

        public ParamValueCollection Values
        {
            get
            {
                if (_values == null)
                    _values = new ParamValueCollection();
                return _values;
            }
            protected set
            {
                _values = value ?? new ParamValueCollection();
            }
        }

        public Token() { }

        public Token(S subscriptionID, P personID, M managerID, string login, string key, DateTimeOffset? expire = null, PersonName name = null, string avatar = "")
        {
            SubscriptionID = subscriptionID;
            PersonID = personID;
            ManagerID = managerID;

            Login = login;
            Key = key;
            Expire = expire;

            Name = name;
            Avatar = avatar;
        }

        public Token(Token<S,P,M> token)
        {
            if (token == null)
                return;

            SubscriptionID = token.SubscriptionID;
            PersonID = token.PersonID;
            ManagerID = token.ManagerID;

            Login = token.Login;
            Key = token.Key;
            Expire = token.Expire;

            Name = token.Name;
            Avatar = token.Avatar;

            Roles = token.Roles == null || token.Roles.Length <= 0 ? null : new List<string>(token.Roles).ToArray();
            Pemissions = token.Pemissions == null || token.Pemissions.Length <= 0 ? null : new List<string>(token.Pemissions).ToArray();

            _values = new ParamValueCollection(token.Values);
        }

        public override string ToString()
        {
            return this.Key;
        }

        public string GetBasicAuthValue()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Login + ":" + this.Key));
        }

        public bool IsInRole(string role)
        {
            if (String.IsNullOrEmpty(role))
                return true;

            if (this.Roles == null)
                return false;

            return this.Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsSupervisor()
        { return this.IsInRole(Supervisor); }

        public bool IsAdministrator()
        { return this.IsInRole(Administrator); }
    }

    public class Token : Token<long, long, long>, ISubscriber, ITokenProvider
    {
        public Token() { }

        public Token(long subscriptionID, long personID, long managerID, string login, string key, DateTimeOffset? expire = null, PersonName name = null, string avatar = "")
            : base(subscriptionID, personID, managerID, login, key, expire, name, avatar)
        { }

        public Token(Token token)
            : base(token)
        { }

        public Token GetToken()
        {
            return new Token(this);
        }
    }
}
