using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ITransferService
    {
        Task<string> UploadFileAsync(string urlApi, User user, int uploadNumber, List<File> files, IProgress<int> progress, string expirationDate = "", string password = "", string phone = "");
        string CreatePassword(int length);
    }
}
