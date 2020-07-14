using OnlineChat.WebApi.Models;
using OnlineChat.WebApi.Models.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services
{
    public class UserService : IUserService
    {
        private IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public User Authenticate(string nickname, string password)
        {
            var user = _userRepo.GetOne(nickname);

            if (user == null || !ValidatePassword(password, user.PasswordHash)) return null;

            return user;
        }

        public User GetUser(string nickname)
        {
            var user = _userRepo.GetOne(nickname);

            return user;
        }

        public User Register(string nickname, string password)
        {
            if(string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password))
            {
                throw new ApplicationException("Nickname and password is required.");
            }

            var isNicknameAlreadyTaken = _userRepo.GetAll().Any(user => user.Nickname == nickname);

            if (isNicknameAlreadyTaken)
            {
                throw new ApplicationException($"Username {nickname} is already taken.");
            }

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            var user = new User()
            {
                Nickname = nickname,
                PasswordHash = Encoding.UTF8.GetString(hash)
            };

            _userRepo.Add(user);

            return user;
        }

        public List<User> SearchForUsers(string nickname)
        {
            return _userRepo.NicknameStartWith(nickname);
        }

        private bool ValidatePassword(string password, string actualHash)
        {
            using var sha256 = SHA256.Create();
            var passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (actualHash == Encoding.UTF8.GetString(passwordHash)) return true;
            else return false;
        }
    }
}
