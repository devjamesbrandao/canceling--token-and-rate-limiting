using Autentication.Core.DTO;
using Autentication.Core.Interfaces;
using Autentication.Core.Models;

namespace Autentication.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        private readonly IJwtHandler _jwtHandler;

        public AccountService(IAccountRepository repository, IJwtHandler jwtHandler)
        {
            _repository = repository;
            _jwtHandler = jwtHandler;
        }

        public async Task SignUp(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception($"Username can not be empty.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception($"Password can not be empty.");
            }
            if (await _repository.GetUserByNameAsync(username) != null)
            {
                throw new Exception($"Username '{username}' is already in use.");
            }

            await _repository.AddUserAsync(
                new UserModelView
                {
                    Password = password,
                    Username = username
                }
            );
        }

        public async Task<JsonWebToken> SignIn(string username, string password)
        {
            var user = await _repository.GetUserByNameAsync(username);

            if (user is null || user.Password != password)
            {
                throw new Exception("Invalid credentials.");
            }

            var jwt = _jwtHandler.GenerateToken(
                new User
                {
                    Password = password,
                    Username = username
                }
            );
            
            return jwt;
        }
    }
}