using System.Security.Cryptography;
using System.Text;

namespace DictionaryApi.Services
{
    /// <summary>
    /// Class HashEncryptor.
    /// </summary>
    public class HashEncryptor
    {
        /// <summary>
        /// Encrypts the with m d5.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string EncryptWithMD5(string value)
        {
            return Encrypt(MD5.Create(), Encoding.ASCII, value);
        }

        /// <summary>
        /// Encrypts the specified algorithm.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="valueToEncrypt">The value to encrypt.</param>
        /// <returns>System.String.</returns>
        public static string Encrypt(HashAlgorithm algorithm, Encoding encoding, string valueToEncrypt)
        {
            var inputBytes = encoding.GetBytes(valueToEncrypt);

            var hash = algorithm.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="stringToEncrypt">The string to encrypt.</param>
        /// <returns>System.String.</returns>
        public static string ComputeHash(string stringToEncrypt)
        {
            return EncryptWithMD5(stringToEncrypt);
        }
    }
}