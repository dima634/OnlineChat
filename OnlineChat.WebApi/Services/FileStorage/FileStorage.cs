using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services.FileStorage
{
    public class FileStorage : IFileStorage
    {
        private static string StoragePath = Directory.GetCurrentDirectory() + "/Blobs";
        private IConfiguration _configuration;

        public FileStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Stream Get(string fileId)
        {
            var path = GetPhysicalPath(fileId);
            return File.OpenRead(path);
        }

        public string Save(string fileId, Stream file)
        {
            var path = GetPhysicalPath(fileId);
            using var physicalFile = File.Create(path);
            file.CopyTo(physicalFile);
            var url = _configuration["AppSettings:HostUrl"] + '/' + fileId;
            return url;
        }

        private string GetPhysicalPath(string fileId) => StoragePath + '/' + fileId;
    }
}
