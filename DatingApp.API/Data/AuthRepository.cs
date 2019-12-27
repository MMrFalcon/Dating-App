using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext dataContext;
        public AuthRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<bool> Exist(string username)
        {
            if (await dataContext.Users.AnyAsync(x => x.Username == username))
                return true;
            
            return false;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedPasword = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                
                for (int i = 0; i < computedPasword.Length; i++)
                {
                    if (computedPasword[i] != passwordHash[i]) return false;
                }

                return true;
            }

        }

        public async Task<User> Register(User user, string password)
        {
            byte[] PasswordHash, PasswordSalt;

            CreatePasswordHash(password, out PasswordHash, out PasswordSalt);

            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PasswordSalt;

            await dataContext.AddAsync(user);
            await dataContext.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}