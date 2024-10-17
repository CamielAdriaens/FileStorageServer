using FileStorage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserByGoogleIdAsync(string googleId, string email, string name);
        Task<List<UserFile>> GetUserFilesAsync(string googleId);
        Task AddUserFileAsync(string googleId, string mongoFileId, string fileName);
    }
}
