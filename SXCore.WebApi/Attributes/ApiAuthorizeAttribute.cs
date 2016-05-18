using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.WebApi.Values;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SXCore.WebApi.Attributes
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    { 
        protected const string BasicAuthResponseHeader = "WWW-Authenticate";
        protected const string BasicAuthLabel = "Basic";
        protected const string TokenAuthLabel = "Token";

        public IAuthenticationProvider AuthProvider { get; set; }
        public ITokenProvider TokenProvider { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (ShouldSkipAuthorization(actionContext))
                    return;

                this.Users = String.IsNullOrEmpty(this.Users) ? this.GetDefaultUsers() : this.Users;
                this.Roles = String.IsNullOrEmpty(this.Roles) ? this.GetDefaultRoles() : this.Roles;


                AuthorizationInput authInput = null;

                // try to get Input from Headres
                var authHeader = actionContext.Request.Headers.Authorization;
                if (authHeader != null && !String.IsNullOrWhiteSpace(authHeader.Parameter))
                    authInput = new AuthorizationInput(authHeader.Scheme, authHeader.Parameter);

                if (authInput == null)
                {
                    // try to get Input from Query
                    var authQueryParam = actionContext.Request.GetQueryNameValuePairs().FirstOrDefault(p => p.Key.Equals(TokenAuthLabel)).Value;
                    if (!String.IsNullOrWhiteSpace(authQueryParam))
                        authInput = new AuthorizationInput(TokenAuthLabel, authQueryParam);
                }

                // input should be provided anyway...
                if (authInput == null)
                {
                    this.Unauthorized(actionContext);
                    return;
                }

                // try to get existing token (token, that was already setted up for current context)
                Token token = this.GetExistingTokenFromTokenProvider(authInput);

                // try to make Authorization with AuthProvider and create new Token
                if (token == null)
                    token = this.MakeNewAuthorizationToken(authInput);

                if (token == null)
                {
                    this.Unauthorized(actionContext);
                    return;
                }

                var principal = new ApiPrincipal(token);

                // setup thread's principal in .NET
                Thread.CurrentPrincipal = principal;

                // setup asp.net user
                if (HttpContext.Current != null)
                    HttpContext.Current.User = principal;

                #region Check Attribute Roles
                if (!String.IsNullOrEmpty(this.Roles))
                {
                    bool inRole = this.Roles.Split(new char[] { ',', ';' }).Any(role => principal.IsInRole(role));
                    if (!inRole)
                    {
                        this.Forbidden(actionContext);
                        return;
                    }
                }
                #endregion

                #region Check Attributes Users
                if (!String.IsNullOrEmpty(this.Users))
                {
                    if (!this.Users.Split(new char[] { ',', ';' }).Contains(principal.Identity.Name.ToString()))
                    {
                        this.Forbidden(actionContext);
                        return;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                this.Exception(actionContext, ex);
            }
        }

        protected Token MakeNewAuthorizationToken(AuthorizationInput authInput)
        {
            // try to make Authorization with AuthProvider and create new Token

            if (this.AuthProvider == null)
                throw new CustomException("AuthenticationProvider is not resolved within Authrorize Attribute!");

            if (authInput.Scheme.Equals(BasicAuthLabel, CommonService.StringComparison))
            {
                // Basic Authentification by Login:Password in base64
                var credentials = this.ParseBasicCredentials(authInput.Value);
                if (credentials != null && credentials.Length == 2)
                    return this.AuthProvider.Authenticate(credentials[0], credentials[1]);
            }
            else if (authInput.Scheme.Equals(TokenAuthLabel, CommonService.StringComparison))
            {
                // Token Authentification by Token
                return this.AuthProvider.Authenticate(authInput.Value);
            }

            return null;
        }

        protected Token GetExistingTokenFromTokenProvider(AuthorizationInput authInput)
        {
            if (this.TokenProvider == null || authInput == null)
                return null;

            // trying to get the existing Token from TokenProvider
            Token token = this.TokenProvider.GetToken();
            if (token == null)
                return null;

            // existing Token should be equal to sended 'Token' in header 
            if (authInput.Scheme.Equals(TokenAuthLabel, CommonService.StringComparison) && token.Key.Equals(authInput.Value, CommonService.StringComparison))
                return token;

            return null;
        }

        protected void Unauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthLabel);
        }

        protected void Exception(HttpActionContext actionContext, Exception ex)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.InternalServerError);
            actionContext.Response.ReasonPhrase = "Authentication System Exception";
            actionContext.Response.Content = new StringContent(ex.Message);
        }

        protected void Forbidden(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthLabel);
        }

        protected string GetDefaultUsers()
        {
            return "";
            //try
            //{
            //    return ConfigurationManager.AppSettings["AuthorizationConfig:Roles"];
            //}
            //catch { return ""; }
        }

        protected string GetDefaultRoles()
        {
            return "";
            //try
            //{
            //    return ConfigurationManager.AppSettings["AuthorizationConfig:Users"];
            //}
            //catch { return ""; }
        }

        protected string[] ParseBasicCredentials(string authHeader)
        {
            try
            {
                string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader)).Split(new[] { ':' });

                if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
                    return null;

                return credentials;
            }
            catch { return null; }
        }

        protected static bool ShouldSkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }

}
