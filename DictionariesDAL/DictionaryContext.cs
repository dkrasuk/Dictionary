using System;
using DictionariesModel;
using System.Configuration;
using System.Data.Entity;
using EntityFramework.Caching;

namespace DictionariesDAL
{
    public class DictionaryContext : DbContext
    {
        public DictionaryContext()
            : base("name=DictionaryContext")
        {
            Configuration.LazyLoadingEnabled = false;

            Database.SetInitializer(new CustomDictionaryInitializer());
        }

        static DictionaryContext()
        {
            CachePolicy.Default.Mode = CacheExpirationMode.Duration;
            CachePolicy.Default.Duration = TimeSpan.FromMinutes(AppSettings.CacheUpdateDurationPeriodInMinutes);
        }

        public DbSet<Dictionary> Dictionaries { get; set; }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(ConfigurationManager.AppSettings["DatabaseSchema"]);
            modelBuilder.Configurations.Add(new Configuration.DictionaryConfiguration());
            modelBuilder.Configurations.Add(new Configuration.ItemConfiguration());
        }
    }
}