using DictionaryApi.Interfaces;
using System;
using System.Net.Http;
using System.Text;

namespace DictionaryApi.Services
{
    /// <summary>
    /// Class AccountService.
    /// </summary>
    /// <seealso cref="DictionaryApi.Interfaces.IAccountService" />
    public class AccountService : IAccountService
    {
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>       
        public AccountService()
        {
            var hrWebApiAddress = AppSettings.HRWebAPIAddress;

            if (hrWebApiAddress[hrWebApiAddress.Length - 1] == ')')
            {
                hrWebApiAddress.Remove(hrWebApiAddress.Length - 1, 1);
            }

            var uri = new Uri(hrWebApiAddress);

            Client.BaseAddress = uri;
            //var byteArray = Encoding.UTF8.GetBytes($"{login}:{password}");
            //Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        /// <summary>
        /// Authenticates the specified login.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="password">The hash.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Authenticate(string login, string password)
        {
            string requestUri =
                $"{Client.BaseAddress}/api/Users/Authenticate?login={login}&hash={HashEncryptor.ComputeHash(password)}";

            var byteArray = Encoding.UTF8.GetBytes($"{login}:{password}");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(byteArray));

            var result = Client.GetAsync(requestUri).Result.ToString();

            return string.Equals(result.ToLower(), "true");
        }
    }
}