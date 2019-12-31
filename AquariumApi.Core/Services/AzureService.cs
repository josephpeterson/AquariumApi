using AquariumApi.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace AquariumApi.Core
{
    public interface IAzureService
    {
        Task UploadFileToStorageContainer(byte[] buffer, string fileName);
        Task UploadFileToStorageContainer(Stream stream, string fileName);
        Task<bool> DeleteFileFromStorageContainer(string path);
        Task<byte[]> GetFileFromStorageContainer(string path);
        bool ExistsInStorageContainer(string path);

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

        public async Task UploadFileToStorageTable(byte[] buffer, string path)
        {
            var share = await GetFileShareTable();

            //Expand paths to share
            var directories = Path.GetDirectoryName(path).Split("\\").ToList();
            var fileName = Path.GetFileName(path);
            directories.Remove(".");

            var currentDir = share.GetRootDirectoryReference();
            foreach(var dir in directories)
            {
                currentDir = currentDir.GetDirectoryReference(dir);
                await currentDir.CreateIfNotExistsAsync();
            }
            await currentDir.GetFileReference(fileName).UploadFromByteArrayAsync(buffer,0,buffer.Length);
        }
        public async Task UploadFileToStorageTable(Stream stream, string path)
        {
            var share = await GetFileShareTable();

            //Expand paths to share
            var directories = Path.GetDirectoryName(path).Split("\\").ToList();
            var fileName = Path.GetFileName(path);
            directories.Remove(".");

            var currentDir = share.GetRootDirectoryReference();
            foreach (var dir in directories)
            {
                currentDir = currentDir.GetDirectoryReference(dir);
                await currentDir.CreateIfNotExistsAsync();
            }
            await currentDir.GetFileReference(fileName).UploadFromStreamAsync(stream);
        }
        public async Task<bool> DeleteFileFromStorageTable(string path)
        {
            var share = await GetFileShareTable();
            var currentDir = share.GetRootDirectoryReference();
            return await currentDir.GetFileReference(path).DeleteIfExistsAsync();
        }
        public async Task<byte[]> GetFileFromStorageTable(string path)
        {
            var share = await GetFileShareTable();
            var currentDir = share.GetRootDirectoryReference();
            using (var ms = new MemoryStream())
            {
                await currentDir.GetFileReference(path.Replace("./","")).DownloadToStreamAsync(ms);
                return ms.ToArray();
            }
        }
        public bool ExistsInStorageTable(string path)
        {
            var share = GetFileShareTable().Result;
            var currentDir = share.GetRootDirectoryReference();
            return currentDir.GetFileReference(path).ExistsAsync().Result;
       }
        private async Task<CloudFileShare> GetFileShareTable()
        {

            string accountName = _configuration["Azure:AccountUsername"];
            string accountKey = _configuration["Azure:AccountKey"];
            string shareName = _configuration["Azure:AquariumPhotosShare"];

            StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            var share = storageAccount.CreateCloudFileClient().GetShareReference(shareName);
            await share.CreateIfNotExistsAsync();
            return share;
        }


        private BlobContainerClient GetFileShareContainer()
        {
            string connectionString = _configuration["Azure:ConnectionString"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            string containerName = "$web";
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            return containerClient;
        }
       
        public async Task UploadFileToStorageContainer(byte[] buffer, string path)
        {
            BlobContainerClient containerClient = GetFileShareContainer();
            BlobClient blobClient = containerClient.GetBlobClient(path);
            using (var stream = new MemoryStream(buffer, writable: false))
            {
                await blobClient.UploadAsync(stream);
            }
        }
        public async Task UploadFileToStorageContainer(Stream stream, string path)
        {
            BlobContainerClient containerClient = GetFileShareContainer();
            BlobClient blobClient = containerClient.GetBlobClient(path);
            await blobClient.UploadAsync(stream);
        }
        public async Task<bool> DeleteFileFromStorageContainer(string path)
        {
            BlobContainerClient containerClient = GetFileShareContainer();
            BlobClient blobClient = containerClient.GetBlobClient(path);
            return await blobClient.DeleteIfExistsAsync();
        }
        public bool ExistsInStorageContainer(string path)
        {
            BlobContainerClient containerClient = GetFileShareContainer();
            BlobClient blobClient = containerClient.GetBlobClient(path);
            return blobClient.Download().GetRawResponse().Status == 200;
        }
        public async Task<byte[]> GetFileFromStorageContainer(string path)
        {
            BlobContainerClient containerClient = GetFileShareContainer();
            BlobClient blobClient = containerClient.GetBlobClient(path);
            var data = await blobClient.DownloadAsync();
            using (var memoryStream = new MemoryStream())
            {
                data.Value.Content.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
