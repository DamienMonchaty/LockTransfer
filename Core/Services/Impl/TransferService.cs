using Core.Models;
using Ionic.Zip;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using File = Core.Models.File;


namespace Core.Services.Impl
{
    public class TransferService : ITransferService
    {
        private RestClient _restClient;

        public TransferService()
        {
            _restClient = new RestSharp.RestClient("https://api.lockself.com/api");
        }

        public string CreatePassword(int length)
        {
            const string alphanumericCharacters =
               "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
               "abcdefghijklmnopqrstuvwxyz" +
               "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        } 

        public async Task<string> UploadFileAsync(string urlApi, User user, int uploadNumber, List<File> files, IProgress<int> progress, string expirationDate = "", string password = "", string phone = "")
        {
            try
            {
                progress.Report(0);

                var request = new RestRequest($"/transfers/password", Method.POST);

                request.AlwaysMultipartFormData = true;

                request.AddHeader("Content-Type", "multipart/form-data");

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Ssl3;

                string access_token = user.token;

                _restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(
                    access_token, "Bearer"
                );

                request.AddHeader("Accept", "application/json");

                using (ZipFile zip = new ZipFile())
                {
                    List<string> filenames = new List<string>();
                    foreach (var file in files)
                    {
                        filenames.Add(file.Title);
                    }
                    zip.AddFiles(filenames, "projet");//Zip file inside filename  

                    string tmpPath = Path.GetTempPath();
                    var tmpZipPath = Path.Combine(tmpPath, "test.zip");

                    zip.Save(tmpZipPath);

                    Console.WriteLine(tmpZipPath);

                    request.AddFile("file", tmpZipPath);
                    request.AddParameter("uuid", Guid.NewGuid().ToString());

                    FileStream fs = new FileStream(tmpZipPath, FileMode.Open);

                    request.AddParameter("totalSize", fs.Length);
                    request.AddParameter("maxDl", uploadNumber);

                    request.AddParameter("password", password);
                    request.AddParameter("phones", phone);

                    int chunckSize = 20971520; //20MB
                    int totalChunks = (int)(fs.Length / chunckSize);
                    if (fs.Length % chunckSize != 0)
                    {
                        totalChunks++;
                    }
                    
                    for (int i = 0; i < totalChunks; i++)
                    {
                        long position = i * (long)chunckSize;
                        int toRead = (int)Math.Min(fs.Length - position, chunckSize);
                        byte[] buffer = new byte[toRead];
                        await fs.ReadAsync(buffer, 0, buffer.Length);

                        request.AddParameter("totalChunkCount", totalChunks);
                        request.AddParameter("chunkIndex", i);

                        if (expirationDate.Length == 0)
                        {
                            expirationDate = "0";
                        }

                        request.AddParameter("expirationDate", expirationDate.ToString());

                        request.AddParameter("ackReceipt", "true");

                        request.AddParameter("isOutlook", "true");

                        progress.Report(20);

                        TransferResponse link = new TransferResponse();

                        var tcs = new TaskCompletionSource<string>();

                        _restClient.ExecuteAsync(
                                   request,
                                   response =>
                                   {
                                       progress.Report(40);

                                       Debug.WriteLine("response -> " + response.Content);
                                       HttpStatusCode statusCode = response.StatusCode;
                                       var statusCodeAsInt = (int)statusCode;

                                       if (statusCodeAsInt >= 400)
                                       {
                                           throw new InvalidOperationException("Request could not be understood by the server: " + response.ErrorMessage,
                                               response.ErrorException);
                                       }
                                       if (statusCodeAsInt >= 401)
                                       {
                                           throw new InvalidOperationException("A server error occurred: " + response.ErrorMessage, response.ErrorException);
                                       }
                                       if (statusCodeAsInt >= 403)
                                       {
                                           throw new InvalidOperationException("Request could not be understood by the server: " + response.ErrorMessage,
                                               response.ErrorException);
                                       }
                                       if (statusCodeAsInt >= 404)
                                       {
                                           throw new InvalidOperationException("Request could not be understood by the server: " + response.ErrorMessage,
                                               response.ErrorException);
                                       }
                                       if (statusCodeAsInt >= 409)
                                       {
                                           throw new InvalidOperationException("Request could not be understood by the server: " + response.ErrorMessage,
                                               response.ErrorException);
                                       }
                                       if (statusCodeAsInt >= 422)
                                       {
                                           throw new InvalidOperationException("Request could not be understood by the server: " + response.ErrorMessage,
                                               response.ErrorException);
                                       }
                                       tcs.SetResult(response.Content);
                                   });
                        progress.Report(60);

                        fs.Close();

                        progress.Report(80);

                        Debug.WriteLine("tcs.Task -> " + tcs.Task.Result);

                        progress.Report(100);

                        return tcs.Task.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("exception service " + ex.Message);
            }
            return null;
        }

        #region Utils
        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
        #endregion
    }
}
