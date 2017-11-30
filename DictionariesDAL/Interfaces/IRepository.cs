using DictionariesModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DictionariesDAL.Interfaces
{
    /// <summary>
    /// Interface IRepository
    /// </summary>
    public interface IRepository
    {
        #region sync
        /// <summary>
        /// Gets all dictionaries.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>IEnumerable&lt;Dictionary&gt;.</returns>
        IEnumerable<Dictionary> GetAllDictionaries(string version);
        /// <summary>
        /// Changes the name, version and metadata of the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Dictionary.</returns>
        Dictionary ChangeDictionary(Dictionary dictionary);

        /// <summary>
        /// Changes the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Item.</returns>
        Item ChangeItem(Item item);

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns>T.</returns>
        T Add<T>(T item) where T : class;

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        void Delete<T>(T item) where T : class;

        /// <summary>
        /// Existses the item.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ExistsItem(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Existses the dictionary.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ExistsDictionary(string name);

        /// <summary>
        /// Gets the dictionary with items.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Dictionary.</returns>
        Dictionary GetDictionaryWithItems(string name, string version);

        /// <summary>
        /// Gets the dictionary without items.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Dictionary.</returns>
        Dictionary GetDictionaryWithoutItems(string name, string version);

        /// <summary>
        /// Gets the dictionaries.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>List&lt;Dictionary&gt;.</returns>
        List<Dictionary> GetDictionaries(string name);

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Item.</returns>
        Item GetItem(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Gets the item with dictionary.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Dictionary.</returns>
        Dictionary GetItemWithDictionary(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Gets the count item.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <returns>System.Int32.</returns>
        int GetCountItem(string dictionaryName, string version);
        #endregion sync
        #region async
        /// <summary>
        /// Gets all dictionaries asynchronous.
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;Dictionary&gt;&gt;.</returns>
        Task<IEnumerable<Dictionary>> GetAllDictionariesAsync();
        /// <summary>
        /// Changes the dictionary name asynchronous.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        Task<Dictionary> ChangeDictionaryAsync(Dictionary dictionary);

        /// <summary>
        /// Changes the item asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        Task<Item> ChangeItemAsync(Item item);

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> AddAsync<T>(T item) where T : class;

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        void DeleteAsync<T>(T item) where T : class;

        /// <summary>
        /// Existses the item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExistsItemAsync(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Existses the dictionary asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExistsDictionaryAsync(string name);

        /// <summary>
        /// Gets the dictionary with items asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        Task<Dictionary> GetDictionaryWithItemsAsync(string name, string version);

        /// <summary>
        /// Gets the dictionary without items asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        Task<Dictionary> GetDictionaryWithoutItemsAsync(string name, string version);

        /// <summary>
        /// Gets the dictionaries asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;List&lt;Dictionary&gt;&gt;.</returns>
        Task<List<Dictionary>> GetDictionariesAsync(string name);

        /// <summary>
        /// Gets the item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        Task<Item> GetItemAsync(string dictionaryName, string version, string valueId);

        /// <summary>
        /// Gets the item with dictionary asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        Task<Dictionary> GetItemWithDictionaryAsync(string dictionaryName, string version, string valueId);

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
