using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public interface IUserRepo : IRepo<User>
    {
        List<User> NicknameStartWith(string str);
    }
}
