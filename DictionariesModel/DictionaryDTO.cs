using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DictionariesModel
{
    public class DictionaryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public JObject Metadata { get; set; }
       
        public virtual ICollection<Item> Items { get; set; }

        public DictionaryDTO()
        {
            Items = new List<Item>();
        }
    }
}