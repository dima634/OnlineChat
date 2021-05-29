using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineChat.WebApi.Models;
using OnlineChat.WebApi.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Controllers
{
    [Route("")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private OnlineChatDatabaseContext _context;
        private IFileStorage _fileStorage;

        public FilesController(IFileStorage fileStorage, OnlineChatDatabaseContext context)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult GetFile([FromRoute] string id)
        {
            var fileContent = _context.FileContents.Single(Content => Content.FileId == Guid.Parse(id));
            var fileStream = _fileStorage.Get(id);
            return File(fileStream, "application/octet-stream", fileContent.Filename);
        }
    }
}
