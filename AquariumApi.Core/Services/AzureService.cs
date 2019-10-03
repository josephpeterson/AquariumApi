using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IAzureService
    {
        Task UploadFileToStorage(byte[] buffer, string fileName);
    }
    public class AzureService : IAzureService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public AzureService(IConfiguration configuration,IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }

        public async Task UploadFileToStorage(byte[] buffer, string path)
        {
            string accountName = _configuration["Azure:AccountUsername"];
            string accountKey = _configuration["Azure:AccountKey"];
            string shareName = _configuration["Azure:AquariumPhotosShare"];

            StorageCredentials storageCredentials = new StorageCredentials(accountName,accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            var share = storageAccount.CreateCloudFileClient().GetShareReference(shareName);
            await share.CreateIfNotExistsAsync();

            //Expand paths to share
            var directories = path.Split("/").ToList();
            var fileName = directories.Last();
            directories.Remove(fileName);

            var currentDir = share.GetRootDirectoryReference();
            directories.ForEach(d =>
            {
                currentDir = currentDir.GetDirectoryReference(d);
                currentDir.CreateIfNotExistsAsync();
            });
            await currentDir.GetFileReference(fileName).UploadFromByteArrayAsync(buffer,0,buffer.Length);
        }
    }
}
