using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Models
{
    public class UserSession
    {
        [JsonProperty(PropertyName = "userId")]
        public int userId { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string username { get; set; }
        [JsonProperty(PropertyName = "userAuth")]
        public string userAuth { get; set; }
        [JsonProperty(PropertyName = "userEmail")]
        public string userEmail { get; set; }
        //[JsonProperty(PropertyName = "organizationNames")]
        //public OrganizationNames organizationNames { get; set; }

        public string Password { get; set; }

        public UserSession(string userEmail, string Password)
        {
            this.userEmail = userEmail;
            this.Password = Password;
        }
    }
}
