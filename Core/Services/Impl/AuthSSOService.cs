using Core.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Contrib;
using NCrontab;
using System.Threading;
using System.Net.Mail;

namespace Core.Services.Impl
{
    public class AuthSSOService : IAuthSSOService
    {
        private RestClient _restClient;
        private string _currentUid = null;

        public AuthSSOService()
        {
        }

        public string GetUrlApi(string email)
        {
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

                string s = JsonConvert.DeserializeObject<string>(response.Content);
                return s;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string FirstStep(string urlApi)
        {
            _restClient = new RestSharp.RestClient(urlApi + "/saml2/login");
            _currentUid = Guid.NewGuid().ToString();
            var request = new RestSharp.RestRequest($"?uuid=" + _currentUid, RestSharp.Method.GET);
            request.AddHeader("Content-Type", "application/json");

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Ssl3;
            try
            {
                var response = _restClient.Execute(request);
                Debug.WriteLine(response.Content);

                BrowserUrl browserUrl = JsonConvert.DeserializeObject<BrowserUrl>(response.Content);
                string url = browserUrl.url;
                return url;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<User> VerifConnect(string urlApi)
        {
            DateTime endTime = DateTime.Now.AddMinutes(5);
            var schedule = CrontabSchedule.Parse("0/10 * * * * *", new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            while (endTime < DateTime.Now)
            {
                var nextRun = schedule.GetNextOccurrence(DateTime.Now);
                if (DateTime.Now > nextRun)
                {
                    var response = PostURL(urlApi);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        nextRun = schedule.GetNextOccurrence(DateTime.Now);
                    }
                    else {

                        User user = JsonConvert.DeserializeObject<User>(response.Content);
                        Debug.WriteLine(user.ToString());
                        Debug.WriteLine(user.token);

                        return user;
                    }
                }
                await Task.Delay(1000);               
            }
            return null;
        }

        public IRestResponse PostURL(string urlApi)
        {
            _restClient = new RestSharp.RestClient(urlApi + "/saml2/connect");

            var request = new RestSharp.RestRequest($"", RestSharp.Method.POST);
            request.AddHeader("Content-Type", "application/json");

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Ssl3;

            string json = JsonConvert.SerializeObject(new
            {
                json = _currentUid
            });

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var response = _restClient.Execute(request);
                Debug.WriteLine(response.Content);

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
