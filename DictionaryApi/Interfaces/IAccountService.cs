namespace DictionaryApi.Interfaces
{
    /// <summary>
    /// Interface IAccountService
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Authenticates the specified login.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="hash">The hash.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Authenticate(string login, string hash);
    }
}
