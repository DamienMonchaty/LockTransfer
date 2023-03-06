using Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Core.Services.Impl
{
    public class AuthService : IAuthService
    {
        private RestClient _restClient;

        public AuthService()
        {
         
        }

        public string GetUrlApi(string email)
        {
            //var uri = new Uri(login);
            //var newQueryString = HttpUtility.ParseQueryString(uri.Query);
            //string query_string = newQueryString[0].ToString();

            MailAddress address = new MailAddress(email);
            string host = address.Host;

            Debug.WriteLine("DOMAINE_CLIENT -> " + host);

            _restClient = new RestSharp.RestClient("https://api.lockself.com/url-api");
            var request = new RestSharp.RestRequest($"?pattern=" + host, RestSharp.Method.GET);
            request.AddHeader("Content-Type", "application/json");

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Ssl3;
            try
            {
                var response = _restClient.Execute(request);
                Debug.WriteLine(response.Content);

                string url = JsonConvert.DeserializeObject<string>(response.Content);
                url = url.Replace("\\", "");

                Debug.WriteLine("URLAPI -> " + url);

                return url;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public User AuthenticateUser(string urlApi, string email, string password)
        {
            _restClient = new RestSharp.RestClient(urlApi + "/api");

            var request = new RestSharp.RestRequest($"/login_check", RestSharp.Method.POST);
            request.AddHeader("Content-Type", "application/json");

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Ssl3;

            string json = JsonConvert.SerializeObject(new
            {
                email = email,
                password = password
            });

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var response = _restClient.Execute(request);
                Debug.WriteLine(response.Content);

                User user = JsonConvert.DeserializeObject<User>(response.Content);
                Debug.WriteLine(user.ToString());
                Debug.WriteLine(user.token);

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Disconnect(string urlApi)
        {
            _restClient = new RestSharp.RestClient(urlApi + "/api");

            var request = new RestSharp.RestRequest($"/disconnect", RestSharp.Method.GET);
            request.AddHeader("Content-Type", "application/json");

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Ssl3;
            try
            {
                _restClient.Execute(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
