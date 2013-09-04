using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudConstruct.SecureFileField.Providers
{
    public class SecureAzureBlobStorageProvider : IStorageProvider
    {
        private CloudStorageAccount accountInfo;
        private CloudBlobClient blobStorage;
        private CloudBlobContainer blobContainer;

        public SecureAzureBlobStorageProvider(string accountName, string accountSharedKey, string blobStorageEndpoint, bool isPrivateContainer)
            : this(accountName, accountSharedKey, blobStorageEndpoint, isPrivateContainer, "secure")
        { }

        public SecureAzureBlobStorageProvider(string accountName, string accountSharedKey, string blobStorageEndpoint, bool isPrivateContainer, string containerName)
        {
            if (accountName == "devstoreaccount1")
            {
                accountInfo = CloudStorageAccount.DevelopmentStorageAccount;
            }
            else
            {
                accountInfo = new CloudStorageAccount(new StorageCredentialsAccountAndKey(accountName, accountSharedKey), false);
            }

            blobStorage = accountInfo.CreateCloudBlobClient();
            blobStorage.RetryPolicy = RetryPolicies.Retry(3, RetryPolicies.DefaultMinBackoff);

            blobContainer = blobStorage.GetContainerReference(containerName);

            EnsureBlobContainer(isPrivateContainer);

        }

        private void EnsureBlobContainer(bool isPrivateContainer)
        {
            blobContainer.CreateIfNotExist();

            BlobContainerPermissions permissions = blobContainer.GetPermissions();
            if (isPrivateContainer)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            }
            else
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            }
            blobContainer.SetPermissions(permissions);
        }

        public bool Delete(string filename)
        {
            CloudBlob blob = blobContainer.GetBlobReference(filename);
            bool result = blob.DeleteIfExists();

            return result;
        }

        public bool Exists(string filename)
        {
            try
            {
                CloudBlob blob = blobContainer.GetBlobReference(filename);
                blob.FetchAttributes();
                return true;
            }
            catch (StorageException e)
            {
                if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public string GetSharedAccessSignature(string filename, int minutes) {

            SharedAccessPolicy sharedAccessPolicy = new SharedAccessPolicy();
            sharedAccessPolicy.Permissions = SharedAccessPermissions.Read;
            sharedAccessPolicy.SharedAccessStartTime = DateTime.UtcNow;
            sharedAccessPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(minutes);

            BlobContainerPermissions blobContainerPermissions = new BlobContainerPermissions();
            blobContainerPermissions.SharedAccessPolicies.Add("default", sharedAccessPolicy);

            blobContainer.SetPermissions(blobContainerPermissions);

            CloudBlob blob = blobContainer.GetBlobReference(filename);
            var sas = blob.GetSharedAccessSignature(new SharedAccessPolicy(), "default");

            return blob.Uri.AbsoluteUri + sas;
        }

        public T Get<T>(string filename) where T : IStorageFile, new()
        {
            CloudBlob blob = blobContainer.GetBlobReference(filename);
            try
            {
                T result = new T();

                blob.FetchAttributes();

                result.ContentType = blob.Properties.ContentType;
                result.ContentLength = blob.Properties.Length;
                result.FileName = filename;

                int length = (int)blob.Properties.Length;
                result.FileBytes = new byte[length];
                Array.Copy(blob.DownloadByteArray(), result.FileBytes, length);

                return result;
            }
            catch
            {
                return default(T);
            }
        }

        public bool Insert(string filename, byte[] buffer, string contentType, long contentLength)
        {
            try
            {
                CloudBlob blob = blobContainer.GetBlobReference(filename);
                blob.Properties.ContentType = contentType;
                
                blob.UploadByteArray(buffer);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Insert(string filename, byte[] buffer, string contentType, long contentLength, bool overwrite)
        {
            CloudBlob blob = blobContainer.GetBlobReference(filename);

            if (!overwrite && blob.Properties.Length > 0)
            {
                return false;
            }

            return Insert(filename, buffer, contentType, contentLength);
        }

        public bool Update(string filename, byte[] buffer, string contentType, long contentLength)
        {
            return Insert(filename, buffer, contentType, contentLength);
        }
    }
}
