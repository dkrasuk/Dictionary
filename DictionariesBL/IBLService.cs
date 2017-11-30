using DictionariesModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DictionariesBL
{
    public interface IBLService
    {
        #region sync
        /// <summary>
        /// Parses the json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>JObject.</returns>
        JObject ParseJson(string value);
        //Dictionary GetDictionary(string name, string version, bool includeItems);
        //string GetItemValue(string dictionaryName, string version, string valueId);
        //Item GetItem(string dictionaryName, string version, string id);
        //Dictionary CreateNewDictionary(string value);
        //Item AddItem(string dictionaryName, string version, string value);
        //Item ChangeItem(string dictionaryName, string version, string id, string value);
        //Dictionary ChangeDictionaryName(string dictionaryName, string version, string value);
        //bool? ItemExists(string dictionaryName, string version, string valueId);
        //bool? DictionaryExists(string dictionaryName);
        //bool DeleteItem(string dictionaryName, string version, string valueId);
        //bool DeleteDictionary(string dictionaryName, string version);
        //int GetCountItem(string dictionaryName, string version);
        #endregion sync

        #region async
        Task<IEnumerable<DictionaryDTO>> GetAllDictionaries();
        /// <summary>
        /// Gets the dictionaries asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <returns>Task&lt;List&lt;Dictionary&gt;&gt;.</returns>
        Task<List<DictionaryDTO>> GetDictionariesAsync(string dictionaryName);

        /// <summary>
        /// Gets the dictionary asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="includeItems">if set to <c>true</c> [include items].</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        Task<DictionaryDTO> GetDictionaryAsync(string dictionaryName, string version, bool includeItems);

        /// <summary>
        /// Gets the item value asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;JObject&gt;.</returns>
        Task<JObject> GetItemValueAsync(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Gets the item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        Task<Item> GetItemAsync(string dictionaryName, string version, string id);

        /// <summary>
        /// Creates the new dictionary asynchronous.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        Task<DictionaryDTO> CreateNewDictionaryAsync(DictionaryDTO dictionary);

        /// <summary>
        /// Adds the item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="value">The value.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        Task<Item> AddItemAsync(string dictionaryName, string version, JObject value);

        /// <summary>
        /// Changes the item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        Task<Item> ChangeItemAsync(string dictionaryName, string version, string id, JObject value);

        /// <summary>
        /// Changes the dictionary name, metadata and version asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        Task<DictionaryDTO> ChangeDictionaryAsync(string dictionaryName, string version, DictionaryDTO dictionary);

        /// <summary>
        /// Items the exists asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;System.Nullable&lt;System.Boolean&gt;&gt;.</returns>
        Task<bool?> ItemExistsAsync(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Dictionaries the exists asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <returns>Task&lt;System.Nullable&lt;System.Boolean&gt;&gt;.</returns>
        Task<bool?> DictionaryExistsAsync(string dictionaryName);

        /// <summary>
        /// Deletes the item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DeleteItemAsync(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Deletes the dictionary asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DeleteDictionaryAsync(string dictionaryName, string version);

        /// <summary>
        /// Gets the count item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> GetCountItemAsync(string dictionaryName, string version);
        #endregion async
    }
}
