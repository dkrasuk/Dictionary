using System.Data.Entity;
using DictionariesModel;

namespace DictionariesDAL
{
    public class CustomDictionaryInitializer : CreateDatabaseIfNotExists<DictionaryContext>
    {
        protected override void Seed(DictionaryContext context)
        {
            context.Dictionaries.Add(new Dictionary()
            {
                Id = 0,
                Items = null,
                Metadata = null,
                Name = "currency",
                Version = "1.0.0"
            });

            context.Items.Add(new Item()
            {
                DictionaryId = 1,
                Value = Newtonsoft.Json.Linq.JObject.Parse("{'id': 1, 'c1': 'UAH','c2': 'гривня'}"),
                ValueId = "1"
            });

            context.Items.Add(
                new Item
                {
                    DictionaryId = 1,
                    Value = Newtonsoft.Json.Linq.JObject.Parse(@"{
                              'id':2,
                              'c1': 'USD',
                              'c2': 'доллар'
                          }"),
                    ValueId = "1"
                });

            context.SaveChanges();
        }
    }
}
