using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DictionariesModel
{
    public class Item
    {
        [JsonIgnore]
        [XmlIgnore]
        public int ItemId { get; set; }
       
        [JsonIgnore]
        [XmlIgnore]
        public int DictionaryId { get; set; }

        [NotMapped]

        public JObject Value { get; set; }

        public string ValueString
        {
            get { return Value.ToString(); }
            set { Value = JObject.Parse(value); }
        }

        public string ValueId
        {
            get
            {
                return
                    (Value.GetValue("id", StringComparison.OrdinalIgnoreCase) ??
                     ((JObject) (Value["id"] = new Guid().ToString())).GetValue("id", StringComparison.OrdinalIgnoreCase))
                        .ToString();
            }
            set
            {

            }
        }
        [JsonIgnore]
        [XmlIgnore]
        public virtual Dictionary Dictionary { get; set; }
    }
}
