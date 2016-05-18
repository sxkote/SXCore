using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Interfaces;
using SXCore.Common.Values;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace SXCore.WebApi
{
    public class ApiPrincipal : IPrincipal, ISubscriber, ITokenProvider
    {
        private IIdentity _identity = null;
        private Token _token = null;

        public IIdentity Identity { get { return _identity; } }
        public Token Token { get { return _token; } }

        public long SubscriptionID { get { return _token == null ? 0 : _token.SubscriptionID; } }
        public long PersonID { get { return _token == null ? 0 : _token.PersonID; } }
        public long ManagerID { get { return _token == null ? 0 : _token.ManagerID; } }

        public PersonName Name { get { return _token?.Name; } }

        public string[] Roles { get { return _token?.Roles; } }

        public ApiPrincipal(Token token)
        {
            if (token == null)
                throw new CustomArgumentException("ApiPrincipal Token is empty");

            _identity = new GenericIdentity(token.Login);
            _token = token;
        }

        public bool IsInRole(string role)
        {
            if (String.IsNullOrEmpty(role))
                return true;

            if (this.Roles == null || this.Roles.Length <= 0)
                return false;

            return this.Roles.Any(r => role.Contains(r));
        }

        public Token GetToken()
        { return _token; }

        static public ApiPrincipal Current
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.User as ApiPrincipal;

                return Thread.CurrentPrincipal as ApiPrincipal;
            }
        }
    }
}
