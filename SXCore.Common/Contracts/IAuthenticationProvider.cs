using SXCore.Common.Values;

namespace SXCore.Common.Contracts
{
    public interface IAuthenticationProvider
    {
        Token Authenticate(string login, string password);
        Token Authenticate(string token);
    }
}
