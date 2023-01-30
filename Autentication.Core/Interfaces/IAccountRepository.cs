using Autentication.Core.DTO;
using Autentication.Core.Models;

namespace Autentication.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<User> GetUserByNameAsync(string username);
        Task AddUserAsync(UserModelView userModel);
    }
}