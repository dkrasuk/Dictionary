using DictionariesDAL.Interfaces;
using DictionariesModel;
using EntityFramework.Caching;
using EntityFramework.Extensions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DictionariesDAL
{
    public class DictionaryRepository : IRepository
    {
        private const string DictionaryTag = "Dictionary";
        #region sync

        /// <summary>
        /// Gets all dictionaries.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>IEnumerable&lt;Dictionary&gt;.</returns>
        public IEnumerable<Dictionary> GetAllDictionaries(string version)
        {
            using (var context = new DictionaryContext())
            {
                return context.Set<Dictionary>()
                    .FromCache(tags: new[] { DictionaryTag })
                    .Where(i => i.Version == version)
                    .ToList();
            }
        }

        /// <summary>
        /// Changes the name, version and metadata of the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Dictionary.</returns>
        public Dictionary ChangeDictionary(Dictionary dictionary)
        {
            using (var dbContext = new DictionaryContext())
            {
                var isNewValue = false;

                var entry = dbContext.Dictionaries.First(t => t.Id == dictionary.Id);

                if (entry == null)
                {
                    isNewValue = true;

                    entry = new Dictionary();
                }

                entry.Name = dictionary.Name;

                entry.Version = dictionary.Version;

                entry.Metadata = dictionary.Metadata;

                if (isNewValue)
                {
                    dbContext.Dictionaries.Add(entry);
                }

                if (dictionary.Items != null && dictionary.Items.Count != 0)
                {
                    dbContext.Items.RemoveRange(
                            dbContext.Items.Where(
                                t => t.DictionaryId == dictionary.Id));

                    foreach (var item in dictionary.Items)
                    {
                        dbContext.Items.Add(new Item
                        {
                            DictionaryId = item.DictionaryId,
                            ItemId = item.ItemId,
                            Value = item.Value,
                            ValueId = item.ValueId,
                            ValueString = item.ValueString
                        });
                    }
                }

                dbContext.SaveChanges();

                RemoveCache();

                return dictionary;
            }
        }

        /// <summary>
        /// Changes the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Item.</returns>
        public Item ChangeItem(Item item)
        {
            using (var dbContext = new DictionaryContext())
            {
                dbContext.Set<Item>().Attach(item);

                var entry = dbContext.Entry(item);

                entry.Property(e => e.Value).IsModified = true;

                dbContext.SaveChanges();

                RemoveCache();

                return item;
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns>T.</returns>
        public T Add<T>(T item) where T : class
        {
            using (var dbContext = new DictionaryContext())
            {
                dbContext.Set<T>().Add(item);

                dbContext.SaveChanges();

                RemoveCache();

                return item;
            }
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public void Delete<T>(T item) where T : class
        {
            using (var dbContext = new DictionaryContext())
            {
                dbContext.Set<T>().Remove(item);

                dbContext.SaveChanges();

                RemoveCache();
            }
        }
        /// <summary>
        /// Existses the item.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ExistsItem(string dictionaryName, string version, string valueId)
        {
            using (var dbContext = new DictionaryContext())
            {
                return dbContext.Set<Item>()
                    .Count(
                        i =>
                            i.Dictionary.Name == dictionaryName && i.Dictionary.Version == version &&
                            i.ValueId == valueId) > 0;
            }
        }

        /// <summary>
        /// Existses the dictionary.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ExistsDictionary(string name)
        {
            using (var dbContext = new DictionaryContext())
            {
                return dbContext.Set<Dictionary>()
                    .FromCache(tags: new[] { DictionaryTag })
                    .Count(x => x.Name == name) > 0;
            }
        }

        /// <summary>
        /// Gets the dictionary with items.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Dictionary.</returns>
        public Dictionary GetDictionaryWithItems(string name, string version)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = dbContext.Set<Dictionary>()
                    .Where(i => i.Name == name && i.Version == version)
                    .Include(x => x.Items)
                    .FromCacheFirstOrDefault(tags: new[] { DictionaryTag });

                return res;
            }
        }

        /// <summary>
        /// Gets the dictionary without items.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Dictionary.</returns>
        public Dictionary GetDictionaryWithoutItems(string name, string version)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = dbContext.Set<Dictionary>()
                    .Where(i => i.Name == name && i.Version == version)
                    .FromCacheFirstOrDefault(tags: new[] { DictionaryTag });

                return res;
            }
        }

        /// <summary>
        /// Gets the dictionaries.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>List&lt;Dictionary&gt;.</returns>
        public List<Dictionary> GetDictionaries(string name)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = dbContext.Set<Dictionary>()
                    .FromCache(tags: new[] { DictionaryTag })
                    .Where(i => i.Name == name)
                    .ToList();

                return res;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Item.</returns>
        public Item GetItem(string dictionaryName, string version, string valueId)
        {
            using (var dbContext = new DictionaryContext())
            {
                var item = dbContext.Set<Item>()
                    .Where(i => i.Dictionary.Name == dictionaryName && i.Dictionary.Version == version && i.ValueId == valueId)
                    .FromCacheFirstOrDefault(tags: new[] { DictionaryTag });

                return item;
            }
        }

        /// <summary>
        /// Gets the item with dictionary.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Dictionary.</returns>
        public Dictionary GetItemWithDictionary(string dictionaryName, string version, string valueId)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = dbContext.Set<Dictionary>()
                    .Where(i => i.Name == dictionaryName && i.Version == version)
                    .Include(x => x.Items.Where(i => i.ValueId == valueId))
                    .FromCacheFirstOrDefault(tags: new[] { DictionaryTag });

                return res;
            }
        }

        /// <summary>
        /// Gets the count item.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <returns>System.Int32.</returns>
        public int GetCountItem(string dictionaryName, string version)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = dbContext
                    .Set<Item>()
                    .FromCache(tags: new[] { DictionaryTag })
                    .Count(i => i.Dictionary.Name == dictionaryName && i.Dictionary.Version == version);

                return res;
            }
        }

        /// <summary>
        /// get all dictionaries as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;Dictionary&gt;&gt;.</returns>
        public async Task<IEnumerable<Dictionary>> GetAllDictionariesAsync()
        {
            using (var context = new DictionaryContext())
            {
                return await context.Set<Dictionary>()
                    .FromCacheAsync(tags: new[] { DictionaryTag });
            }
        }
        #endregion sync

        #region async
        /// <summary>
        /// change dictionary version as an asynchronous operation.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        public async Task<Dictionary> ChangeDictionaryAsync(Dictionary dictionary)
        {
            using (var dbContext = new DictionaryContext())
            {
                var isNewValue = false;

                var entry = dbContext.Dictionaries.First(t => t.Id == dictionary.Id);

                if (entry == null)
                {
                    isNewValue = true;

                    entry = new Dictionary();
                }

                entry.Name = dictionary.Name;

                entry.Version = dictionary.Version;

                entry.Metadata = dictionary.Metadata;

                if (isNewValue)
                {
                    dbContext.Dictionaries.Add(entry);
                }

                if (dictionary.Items != null && dictionary.Items.Count != 0)
                {
                    dbContext.Items.RemoveRange(
                            dbContext.Items.Where(
                                t => t.DictionaryId == dictionary.Id));

                    foreach (var item in dictionary.Items)
                    {
                        dbContext.Items.Add(new Item
                        {
                            DictionaryId = item.DictionaryId,
                            ItemId = item.ItemId,
                            Value = item.Value,
                            ValueId = item.ValueId,
                            ValueString = item.ValueString
                        });
                    }
                }

                await dbContext.SaveChangesAsync();

                RemoveCache();

                return dictionary;
            }
        }

        /// <summary>
        /// change item as an asynchronous operation.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Task&lt;Item&gt;.</returns>
        public async Task<Item> ChangeItemAsync(Item item)
        {
            using (var dbContext = new DictionaryContext())
            {
                dbContext.Set<Item>().Attach(item);

                var entry = dbContext.Entry(item);

                entry.Property(e => e.ValueString).IsModified = true;

                await dbContext.SaveChangesAsync();

                RemoveCache();

                return item;
            }
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns>System.Threading.Tasks.Task&lt;T&gt;.</returns>
        public async Task<T> AddAsync<T>(T item) where T : class
        {
            using (var dbContext = new DictionaryContext())
            {
                dbContext.Set<T>().Add(item);

                await dbContext.SaveChangesAsync();

                RemoveCache();

                return item;
            }
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public async void DeleteAsync<T>(T item) where T : class
        {
            using (var dbContext = new DictionaryContext())
            {
                var entry = dbContext.Entry(item);

                if (entry.State == EntityState.Detached)
                {
                    dbContext.Set<T>().Attach(item);
                }

                dbContext.Set<T>().Remove(item);

                await dbContext.SaveChangesAsync();

                RemoveCache();
            }
        }

        /// <summary>
        /// exists item as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ExistsItemAsync(string dictionaryName, string version, string valueId)
        {
            using (var dbContext = new DictionaryContext())
            {
                return await dbContext.Set<Item>()
                    .AnyAsync(i => i.Dictionary.Name == dictionaryName && i.Dictionary.Version == version && i.ValueId == valueId);
            }
        }

        /// <summary>
        /// exists dictionary as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ExistsDictionaryAsync(string name)
        {
            using (var dbContext = new DictionaryContext())
            {
                var result = await dbContext.Set<Dictionary>()
                        .FromCacheAsync(tags: new[] { DictionaryTag });

                return result.Any(x => x.Name == name);
            }
        }

        /// <summary>
        /// get dictionary with items as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        public async Task<Dictionary> GetDictionaryWithItemsAsync(string name, string version)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = await dbContext.Set<Dictionary>()
                    .Include(x => x.Items)
                    .Where(i => i.Name == name && i.Version == version)
                    .FromCacheFirstOrDefaultAsync(tags: new[] { DictionaryTag });

                return res;
            }
        }

        /// <summary>
        /// get dictionary without items as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        public async Task<Dictionary> GetDictionaryWithoutItemsAsync(string name, string version)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = await dbContext.Set<Dictionary>()
                    .Where(i => i.Name == name && i.Version == version)
                    .FromCacheFirstOrDefaultAsync(tags: new[] { DictionaryTag });

                return res;
            }
        }

        /// <summary>
        /// get dictionaries as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;List&lt;Dictionary&gt;&gt;.</returns>
        public async Task<List<Dictionary>> GetDictionariesAsync(string name)
        {
            using (var dbContext = new DictionaryContext())
            {
                var dictionaries = await dbContext.Dictionaries
                    .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                    .FromCacheAsync(tags: new[] {DictionaryTag});

                foreach (var dictionary in dictionaries)
                {
                    dictionary.Items = (from i in dbContext.Items.FromCache(tags: new[] {DictionaryTag})
                        where i.DictionaryId == dictionary.Id
                        select new Item
                        {
                            DictionaryId = dictionary.Id,
                            ItemId = i.ItemId,
                            ValueId = i.ValueId,
                            ValueString = i.ValueString
                        }).ToList();
                }

                return dictionaries as List<Dictionary>;
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
            using (var dbContext = new DictionaryContext())
            {
                var item = await dbContext.Set<Item>()
                    .Where(i => i.Dictionary.Name == dictionaryName && i.Dictionary.Version == version && i.ValueId == valueId)
                    .FromCacheFirstOrDefaultAsync(tags: new[] { DictionaryTag });

                return item;
            }
        }

        /// <summary>
        /// get item with dictionary as an asynchronous operation.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="valueId">The value identifier.</param>
        /// <returns>Task&lt;Dictionary&gt;.</returns>
        public async Task<Dictionary> GetItemWithDictionaryAsync(string dictionaryName, string version, string valueId)
        {
            using (var dbContext = new DictionaryContext())
            {
                var res = await dbContext.Set<Dictionary>()
                    .Where(i => i.Name == dictionaryName && i.Version == version)
                    .Include(x => x.Items.Where(i => i.ValueId == valueId))
                    .FromCacheFirstOrDefaultAsync(tags: new[] { DictionaryTag });

                return res;
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
            using (var dbContext = new DictionaryContext())
            {
                var res = await dbContext.Set<Item>()
                    .Where(i => i.Dictionary.Name == dictionaryName && i.Dictionary.Version == version)
                    .CountAsync();

                return res;
            }
        }
        #endregion async

        private static void RemoveCache()
        {
            CacheManager.Current.Expire(DictionaryTag);
        }
    }
}
