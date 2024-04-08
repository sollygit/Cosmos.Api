using Cosmos.Api.Configurations;
using Cosmos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Api.Services
{
    public interface IStorageService
    {
        Task<string> UploadFromStreamAsync(IFormFile file);
        Task <IEnumerable<UploadedFile>> GetBlobItemsAsync();
        Task<UploadedFile> GetBlobItemAsync(string name);
        Task<bool> DeleteIfExistsAsync(string name);
    }

    public class BlobStorageService : IStorageService
    {
        private readonly StorageConfig _config;
        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _client;
        private readonly CloudBlobContainer _container;

        public BlobStorageService(IOptions<StorageConfig> config)
        {
            _config = config.Value;
            _account = CloudStorageAccount.Parse(_config.ConnectionString);
            _client = _account.CreateCloudBlobClient();
            _container = _client.GetContainerReference("blober");
        }

        public async Task<string> UploadFromStreamAsync(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}".ToUpper();
            var blob = _container.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(file.OpenReadStream());
            return fileName;
        }

        public async Task<IEnumerable<UploadedFile>> GetBlobItemsAsync()
        {
            var token = new BlobContinuationToken();
            var blobList = await _container.ListBlobsSegmentedAsync(token);
            var items = blobList.Results
                .Select(o => o as CloudBlockBlob)
                .Select(o => {
                    return new UploadedFile
                    {
                        Name = o.Name,
                        Uri = o.Uri.ToString(),
                        LastModified = o.Properties.LastModified
                    };
                });
            return items;
        }

        public async Task<UploadedFile> GetBlobItemAsync(string name)
        {
            var token = new BlobContinuationToken();
            var blobList = await _container.ListBlobsSegmentedAsync(token);
            var item = blobList.Results
                .Select(o => o as CloudBlockBlob)
                .Where(o => o.Name == name)
                .FirstOrDefault();

            if (item == null) return null;

            return new UploadedFile {
                Name = item?.Name,
                Uri = item?.Uri.ToString(),
                LastModified = item?.Properties.LastModified
            };
        }

        public async Task<bool> DeleteIfExistsAsync(string name)
        {
            var token = new BlobContinuationToken();
            var blobList = await _container.ListBlobsSegmentedAsync(token);
            var item = blobList.Results
                .Select(o => o as CloudBlockBlob)
                .Where(o => o.Name == name)
                .FirstOrDefault();

            if (item == null) return false;
            return await item.DeleteIfExistsAsync();
        }
    }
}
