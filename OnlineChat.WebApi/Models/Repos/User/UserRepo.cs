using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public class UserRepo : BaseRepo<User>, IUserRepo
    {
        public UserRepo(OnlineChatDatabaseContext db): base(db)
        {
            Table = Context.Users;
        }

        public List<User> NicknameStartWith(string str)
            => Table.Where(user => user.Nickname.StartsWith(str)).ToList();
    }
}
