using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TransferResponse
    {
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
        [JsonProperty(PropertyName = "downloadLink")]
        public string downloadLink { get; set; }
        [JsonProperty(PropertyName = "reportLink")]
        public string reportLink { get; set; }

    }
}
