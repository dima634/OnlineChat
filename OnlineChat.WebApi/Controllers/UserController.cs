using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineChat.WebApi.Services;

namespace OnlineChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("search/{username}")]
        public ActionResult<IEnumerable<string>> SearchForUser([FromRoute] string username)
        {
            var suggestions = _userService.SearchForUsers(username);
            return Ok(suggestions.Select(user => user.Nickname));
        }
    }
}
