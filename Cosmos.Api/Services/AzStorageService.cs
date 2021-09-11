using Cosmos.Common;
using Cosmos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        Task<string> UploadAsync(IFormFile file);
        Task <IEnumerable<UploadedFile>> GetBlobItemsAsync();
    }

    public class AzStorageService : IStorageService
    {
        readonly string _connectionString;
        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _client;
        private readonly CloudBlobContainer _container;

        public AzStorageService(IConfiguration config)
        {
            _connectionString = config["StorageConnectionString"];
            _account = CloudStorageAccount.Parse(_connectionString);
            _client = _account.CreateCloudBlobClient();
            _container = _client.GetContainerReference("blober");
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var id = Guid.NewGuid();
            var fileName = $"{Constants.RAW_PREFIX}_{id}{Path.GetExtension(file.FileName)}".ToUpper();
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
                        FileName = o.Name,
                        Uri = o.Uri.ToString(),
                        LastModified = o.Properties.LastModified
                    };
                });
            return items;
        }
    }
}
