using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using AlfaBank.Logger;
using DictionaryApi.Interfaces;

namespace DictionaryApi.Filters
{

    /// <summary>
    /// Class ApiAuthenticationFilter.
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.AuthorizationFilterAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiAuthenticationFilter : AuthorizationFilterAttribute
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiAuthenticationFilter"/> class.
        /// </summary>
        public ApiAuthenticationFilter()
        {
            _accountService = DependencyResolver.Current.GetService<IAccountService>();
        }
        /// <summary>
        /// Called when authorization has occurred.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            if (!IsAuthenticate(filterContext))
            {
                ChallengeAuthRequest(filterContext);
                return;
            }
            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// Determines whether the specified filter context is authenticate.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <returns>
        ///   <c>true</c> if the specified filter context is authenticate; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsAuthenticate(HttpActionContext filterContext)
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();

            var authRequest = filterContext.Request.Headers.Authorization?.ToString() ?? string.Empty;

            var decodedAuth = Encoding.Default.GetString(
                Convert.FromBase64String(
                    authRequest.Replace("Basic ", string.Empty)));

            var delimeterIndex = decodedAuth.IndexOf(":", StringComparison.Ordinal);

            if (delimeterIndex == -1)
            {
                logger.Warning($"IsAuthenticate wrong auth header: {authRequest}");

                return false;
            }

            var login = decodedAuth.Substring(0, delimeterIndex);

            var pass = decodedAuth.Substring(delimeterIndex + 1, decodedAuth.Length - delimeterIndex - 1);

            logger.Info($"IsAuthenticate login: {login}");

            var authResult = _accountService.Authenticate(login, pass);

            if (!authResult)
            {
                logger.Warning($"Incorrect login/pass [login: {login}]");
            }

            return authResult;
        }

        /// <summary>
        /// Challenges the authentication request.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        private static void ChallengeAuthRequest(HttpActionContext filterContext)
        {
            var dnsHost = filterContext.Request.RequestUri.DnsSafeHost;

            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            filterContext.Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"{dnsHost}\"");
        }

      
    }
}