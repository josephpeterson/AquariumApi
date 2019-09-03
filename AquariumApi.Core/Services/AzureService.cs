using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AquariumApi.Core.Services
{
    public interface IAzureService
    {

    }
    class AzureService : IAzureService
    {

        public async Task UploadToBlobStorageAsync()
        {
            /*
            string storageConnection = CloudConfigurationManager.GetSetting("BlobStorageConnectionString");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);

            //create a block blob CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            //create a container CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("appcontainer");

            //create a container if it is not already exists

            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("appcontainer");

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {

                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            }

            string imageName = "Test-" + Path.GetExtension(imageToUpload.FileName);

            //get Blob reference

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName); cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);
            */
        }
    }
}
