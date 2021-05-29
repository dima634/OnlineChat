using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services.FileStorage
{
    public interface IFileStorage
    {
        /// <returns>File url</returns>
        string Save(string fileId, Stream file);
        Stream Get(string fileId);
    }
}
