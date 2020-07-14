using OnlineChat.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services
{
    public interface IUserService
    {
        User Authenticate(string nickname, string password);
        User Register(string nickname, string password);
        User GetUser(string nickname);
        List<User> SearchForUsers(string nickname);
    }
}
