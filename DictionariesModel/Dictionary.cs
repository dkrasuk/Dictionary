using System.Collections.Generic;

namespace DictionariesModel
{
    public class Dictionary
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string Metadata { get; set; }
       
        public virtual ICollection<Item> Items { get; set; }

        public Dictionary()
        {
            Items = new List<Item>();
        }
    }
}
