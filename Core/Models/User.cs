using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "token")]
        public string token { get; set; }
        [JsonProperty(PropertyName = "userSession")]
        public UserSession userSession { get; set; }
    }

}
