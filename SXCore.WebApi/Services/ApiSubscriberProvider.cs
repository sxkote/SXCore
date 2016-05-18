using SXCore.Common.Contracts;
using SXCore.Common.Interfaces;
using SXCore.Common.Values;

namespace SXCore.WebApi.Services
{
    public class ApiSubscriberProvider : ISubscriber, ITokenProvider
    {
        public ApiPrincipal CurrentPrincipal
        { get { return ApiPrincipal.Current; } }

        public long SubscriptionID
        {
            get
            {
                try
                {
                    var principal = this.CurrentPrincipal;
                    return principal == null ? 0 : principal.SubscriptionID;
                }
                catch
                {
                    throw;
                }
            }
        }

        public long ManagerID
        {
            get
            {
                try
                {
                    var principal = this.CurrentPrincipal;
                    return principal == null ? 0 : principal.ManagerID;
                }
                catch
                {
                    throw;
                }
            }
        }

        public long PersonID
        {
            get
            {
                try
                {
                    var principal = this.CurrentPrincipal;
                    return principal == null ? 0 : principal.PersonID;
                }
                catch
                {
                    throw;
                }
            }
        }

        public Token GetToken()
        { return this.CurrentPrincipal?.Token; }
    }

}
