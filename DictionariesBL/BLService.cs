using DictionariesDAL.Interfaces;
using DictionariesModel;
using AlfaBank.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DictionariesBL
{
    public class BLService : IBLService
    {
        private readonly IRepository _dataRepository;
        private readonly ILogger _logger;
        private const int DuplicateKeyNumber = 2601;

        public BLService(IRepository db, ILogger logger)
        {
            _dataRepository = db;
            _logger = logger;
        }
        #region sync
        /// <summary>
        /// Parses the json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>JObject.</returns>
        public JObject ParseJson(string value)
        {
            JObject itemValue = null;

            try
            {
                itemValue = JObject.Parse(value);
            }
            catch (JsonReaderException ex)
            {
                _logger.Error($"ParseJson JsonReaderException:{ex}  value:{value}");

            }
            catch (Exception ex)
            {
                _logger.Error($"ParseJson Exception:{ex}  value:{value}");
                throw;
            }
            return itemValue;
        }
        #endregion #region sync

        #region async
        /// <summary>
        /// Gets all dictionaries.
        /// </summary>
        /// <returns>System.Threading.Tasks.Task&lt;System.Collections.Generic.IEnumerable&lt;DictionariesModel.Dictionary&gt;&gt;.</returns>
        public async Task<IEnumerable<DictionaryDTO>> GetAllDictionaries()
        {
            _logger.Info("BLService.GetAllDictionaries");

            var dictionaries = await _dataRepository.GetAllDictionariesAsync();
            var result = dictionaries.ToList();
            
            return result.Select(ConvertToDictionaryDto).ToList();
        }

        /// <summary>
        /// add item as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="value">The value.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        /// <exception cref="System.ArgumentException">
        /// DictionaryName or version or value cannot be null
        /// or
        /// </exception>
        /// <exception cref="System.Exception">Cannot insert duplicate</exception>
        public async Task<Item> AddItemAsync(string dictionaryName, string version, JObject value)
        {
            _logger.Info($"BLService.AddItem [DictionaryName: {dictionaryName}, Version: {version}, Value:{value}]");

            if (string.IsNullOrEmpty(dictionaryName) || string.IsNullOrEmpty(version) || value == null)
            {
                throw new ArgumentException("DictionaryName or version or value cannot be null");
            }

            var dict = await _dataRepository.GetDictionaryWithoutItemsAsync(dictionaryName, version);

            if (dict == null)
            {
                throw new ArgumentException($"Dictionary {dictionaryName} version {version} is not exists ");
            }

            try
            {
                var itemValue = value;
                var item = new Item
                {
                    DictionaryId = dict.Id,
                    Value = itemValue
                };

                return await _dataRepository.AddAsync(item);
            }
            catch (SqlException ex)
            {
                if (ex.Number == DuplicateKeyNumber)
                {
                    throw new Exception("Cannot insert duplicate");
                }

                _logger.Error($"AddItem Exception:{ex}  value:{value}");

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"AddItemAsync Exception:{ex}  value:{value}");

                throw;
            }
        }

        /// <summary>
        /// Change or create new dictionary.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>System.Threading.Tasks.Task&lt;DictionariesModel.Dictionary&gt;.</returns>
        public async Task<DictionaryDTO> ChangeDictionaryAsync(
            string dictionaryName, string version, DictionaryDTO dictionary)
        {
            _logger.Info($"BLService.ChangeDictionaryNameAsync [" +
                         $"DictionaryName: {dictionaryName}, Version: {version}, Value:{dictionary}]");

            try
            {
                var oldDict = await _dataRepository.GetDictionaryWithoutItemsAsync(dictionaryName, version);

                if (oldDict == null)
                {
                    return await CreateNewDictionaryAsync(dictionary);
                }

                oldDict.Name = dictionary.Name;

                oldDict.Metadata = dictionary.Metadata?.ToString();

                oldDict.Version = dictionary.Version;

                if (dictionary.Items != null && dictionary.Items.Count != 0)
                {
                    oldDict.Items = dictionary.Items;

                    foreach (var oldDictItem in oldDict.Items)
                    {
                        oldDictItem.DictionaryId = oldDict.Id;
                    }
                }
                var changedDictionary = await _dataRepository.ChangeDictionaryAsync(oldDict);
                
                return ConvertToDictionaryDto(changedDictionary);
            }
            catch (SqlException ex)
            {
                if (ex.Number == DuplicateKeyNumber)
                {
                    throw new Exception("Cannot insert duplicate");
                }

                _logger.Error($"ChangeDictionaryName Exception:{ex}  value:{dictionary}");

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateNewDictionary Exception:{ex}  value:{dictionary}");

                throw;
            }
        }

        /// <summary>
        /// Changes the item asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Threading.Tasks.Task&lt;DictionariesModel.Item&gt;.</returns>
        public async Task<Item> ChangeItemAsync(string dictionaryName, string version, string id, JObject value)
        {
            _logger.Info($"BLService.ChangeItem [DictionaryName: {dictionaryName}, Version: {version}, Id: {id}, Value: {value}]");

            try
            {
                var valueId = value.Property("id");

                if (valueId == null)
                {
                    valueId = new JProperty("id", id);
                    value.Add(valueId);
                }
                else
                {
                    valueId.Value = id;
                }
                var item = await GetItemAsync(dictionaryName, version, id);

                if (item == null)
                {
                    throw new ApplicationException(
                        $"Can't find item '{id}' in dictionary '{dictionaryName}' with version '{version}'");
                }

                item.Value = value;

                return await _dataRepository.ChangeItemAsync(item);
            }
            catch (SqlException ex)
            {
                if (ex.Number == DuplicateKeyNumber)
                {
                    throw new ArgumentException("Cannot insert duplicate");
                }

                _logger.Error($"ChangeItemAsync Exception:{ex}  value:{value}");

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"ChangeItemAsync Exception:{ex}  value:{value}");

                throw;
            }
        }

        /// <summary>
        /// create new dictionary as an asynchronous operation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        /// <exception cref="System.Exception">Cannot insert duplicate</exception>
        public async Task<DictionaryDTO> CreateNewDictionaryAsync(DictionaryDTO dictionary)
        {
            _logger.Info($"BLService.CreateNewDictionary [Value:{dictionary}]");

            try
            {
                var dictionaryForSave = new Dictionary
                {
                    Id = dictionary.Id,
                    Items = dictionary.Items,
                    Metadata = dictionary.Metadata?.ToString(),
                    Name = dictionary.Name,
                    Version = dictionary.Version
                };
                var result = await _dataRepository.AddAsync(dictionaryForSave);
                return ConvertToDictionaryDto(result);
            }
            catch (SqlException ex)
            {
                if (ex.Number == DuplicateKeyNumber)
                {
                    throw new Exception("Cannot insert duplicate");
                }

                _logger.Error($"CreateNewDictionary Exception:{ex}  value:{dictionary}");

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateNewDictionary Exception:{ex}  value:{dictionary}");

                throw;
            }
        }

        /// <summary>
        /// delete dictionary as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> DeleteDictionaryAsync(string dictionaryName, string version)
        {
            _logger.Info($"BLService.DeleteDictionary [DictionaryName: {dictionaryName}, Version: {version}]");

            try
            {
                var dict =  await _dataRepository.GetDictionaryWithoutItemsAsync(dictionaryName, version);

                if (dict == null)
                {
                    return false;
                }
                _dataRepository.DeleteAsync(dict);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteDictionary Exception:{ex}  dictionaryName:{dictionaryName}");

                throw;
            }
        }

        /// <summary>
        /// delete item as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> DeleteItemAsync(string dictionaryName, string version, string valueId)
        {
            _logger.Info($"BLService.DeleteItem [DictionaryName: {dictionaryName}, Version: {version}, Value:{valueId}]");

            try
            {
                var item = await _dataRepository.GetItemAsync(dictionaryName, version, valueId);

                _dataRepository.DeleteAsync(item);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteDictionary Exception:{ex}  dictionaryName:{dictionaryName} value:{valueId}");

                throw;
            }
        }

        /// <summary>
        /// Dictionaries the exists asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Nullable&lt;System.Boolean&gt;&gt;.</returns>
        public async Task<bool?> DictionaryExistsAsync(string dictionaryName)
        {
            _logger.Info($"BLService.DictionaryExists [DictionaryName: {dictionaryName}]");

            try
            {
                return await _dataRepository.ExistsDictionaryAsync(dictionaryName);
            }
            catch (Exception ex)
            {
                _logger.Error($"DictionaryExists Exception:{ex}  dictionaryName:{dictionaryName}");

                throw;
            }

        }

        /// <summary>
        /// get dictionary as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="includeItems">if set to <c>true</c> [include items].</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        public async Task<DictionaryDTO> GetDictionaryAsync(string dictionaryName, string version, bool includeItems)
        {
            _logger.Info($"BLService.GetDictionary [" +
                         $"DictionaryName: {dictionaryName}, Version: {version}, IncludeItems: {includeItems}]");

            try
            {
                Dictionary dictionary = null;
                if (includeItems)
                {
                    dictionary = await _dataRepository.GetDictionaryWithItemsAsync(dictionaryName, version);
                }
                else
                {
                    dictionary = await _dataRepository.GetDictionaryWithoutItemsAsync(dictionaryName, version);
                }

                if (dictionary != null)
                {
                    return ConvertToDictionaryDto(dictionary);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetDictionary Exception:{ex}  " +
                              $"dictionaryName:{dictionaryName} version: {version} includeItems:{includeItems}");

                throw;
            }
        }

        /// <summary>
        /// get item as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        public async Task<Item> GetItemAsync(string dictionaryName, string version, string valueId)
        {
            _logger.Info($"BLService.GetItem [DictionaryName: {dictionaryName}, Version: {version}, Value:{valueId}]");

            try
            {
                return await _dataRepository.GetItemAsync(dictionaryName, version, valueId);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetItem Exception:{ex}  dictionaryName:{dictionaryName} version: {version} valueId:{valueId}");

                throw;
            }
        }

        /// <summary>
        /// get item value as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;JObject&gt;.</returns>
        public async Task<JObject> GetItemValueAsync(string dictionaryName, string version, string valueId)
        {
            _logger.Info($"BLService.GetItemValue [DictionaryName: {dictionaryName}, Version: {version}, Value:{valueId}]");

            try
            {
                var item = await GetItemAsync(dictionaryName, version, valueId);

                return item?.Value;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetItemValue Exception:{ex}  dictionaryName:{dictionaryName} version: {version} valueId:{valueId}");

                throw;
            }
        }

        /// <summary>
        /// Items the exists asynchronous.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Nullable&lt;System.Boolean&gt;&gt;.</returns>
        public async Task<bool?> ItemExistsAsync(string dictionaryName, string version, string valueId)
        {
            _logger.Info($"BLService.ItemExists [DictionaryName: {dictionaryName}, Version: {version}, Value:{valueId}]");

            try
            {
                return await _dataRepository.ExistsItemAsync(dictionaryName, version, valueId);
            }
            catch (Exception ex)
            {

                _logger.Error($"ItemExists Exception:{ex}  dictionaryName:{dictionaryName} version: {version} valueId:{valueId}");

                throw;
            }
        }

        /// <summary>
        /// get count item as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> GetCountItemAsync(string dictionaryName, string version)
        {
            _logger.Info($"BLService.GetCountItem [DictionaryName: {dictionaryName}, Version: {version}]");

            try
            {
                return await _dataRepository.GetCountItemAsync(dictionaryName, version);
            }
            catch (Exception ex)
            {
                _logger.Error($"ItemExists Exception:{ex}  dictionaryName:{dictionaryName} version: {version}");

                throw;
            }
        }

        /// <summary>
        /// get dictionaries as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <returns>Task&lt;List&lt;Dictionary&gt;&gt;.</returns>
        public async Task<List<DictionaryDTO>> GetDictionariesAsync(string dictionaryName)
        {
            _logger.Info($"BLService.GetDictionaries [DictionaryName: {dictionaryName}]");

            try
            {
                var dictionaries = await _dataRepository.GetDictionariesAsync(dictionaryName);
                var dictionariesList = dictionaries.ToList();
                return dictionariesList.Select(ConvertToDictionaryDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error($"GetDictionariesAsync Exception:{ex}  dictionaryName:{dictionaryName}");

                throw;
            }
        }
        #endregion async

        private DictionaryDTO ConvertToDictionaryDto(Dictionary dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }
            return new DictionaryDTO
            {
                Id = dictionary.Id,
                Items = dictionary.Items,
                Metadata = string.IsNullOrEmpty(dictionary.Metadata) ? null : JObject.Parse(dictionary.Metadata),
                Name = dictionary.Name,
                Version = dictionary.Version
            };
        }
    }
}
