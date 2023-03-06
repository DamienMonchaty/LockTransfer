using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Services
{
    public interface IAuthService
    {
        string GetUrlApi(string email);
        User AuthenticateUser(string urlApi, string email, string password);
        void Disconnect(string urlApi);
    }
}
