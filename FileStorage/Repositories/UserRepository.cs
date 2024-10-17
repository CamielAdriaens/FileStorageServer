using FileStorage.Database;
using FileStorage.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileStorage.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByGoogleId(string googleId)
        {
            return await _context.Users
                .Include(u => u.UserFiles)
                .FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }

        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task AddUserFile(UserFile file)
        {
            _context.UserFiles.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserFile>> GetUserFiles(string googleId)
        {
            var user = await _context.Users
                .Include(u => u.UserFiles)
                .FirstOrDefaultAsync(u => u.GoogleId == googleId);

            return user?.UserFiles ?? new List<UserFile>();
        }
    }
}
