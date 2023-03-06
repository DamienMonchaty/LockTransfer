using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class BrowserUrl
    {
        [JsonProperty(PropertyName = "url")]
        public string url { get; set; }
    }
}
