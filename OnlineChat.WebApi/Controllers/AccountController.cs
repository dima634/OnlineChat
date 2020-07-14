using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineChat.Dtos.BindingModels;
using OnlineChat.WebApi.Helpers;
using OnlineChat.WebApi.Services;

namespace OnlineChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IUserService _userService;
        private AppSettings _appSettings;

        public AccountController(IUserService userService, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("register")]
        public ActionResult Register([FromBody] RegisterModel model)
        {
            try
            {
                _userService.Register(model.Username, model.Password);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok();
        }

        [HttpPost]
        [Route("token")]
        public ActionResult Token([FromBody] AuthorizeModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if(user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.EncryptionKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, model.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }
}