using Core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IAuthSSOService
    {
        string GetUrlApi(string email);
        string FirstStep(string email);
        Task<User> VerifConnect(string email);
        IRestResponse PostURL(string email);
    }
}
