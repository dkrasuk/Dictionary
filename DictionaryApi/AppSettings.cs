using System.Configuration;

namespace DictionaryApi
{
    /// <summary>
    /// Class AppSettings.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets the hr web API address.
        /// </summary>
        /// <value>The hr web API address.</value>
        public static string HRWebAPIAddress => ConfigurationManager.AppSettings["HRWebAPIAddress"];

        /// <summary>
        /// Gets the hr web API login.
        /// </summary>
        /// <value>The hr web API login.</value>
        public static string HRWebAPILogin => ConfigurationManager.AppSettings["HRWebAPILogin"];

        /// <summary>
        /// Gets the hr web API password.
        /// </summary>
        /// <value>The hr web API password.</value>
        public static string HRWebAPIPassword => ConfigurationManager.AppSettings["HRWebAPIPassword"];
    }
}