﻿using FileStorage.Database;
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
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // This line saves the changes to the SQL database
                Console.WriteLine($"User with Google ID {user.GoogleId} saved to database.");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user to database: {ex.Message}");
                throw;
            }
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

        public async Task RemoveUserFile(int userId, string mongoFileId)
        {
            var userFile = await _context.UserFiles
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MongoFileId == mongoFileId);

            if (userFile != null)
            {
                _context.UserFiles.Remove(userFile);
                await _context.SaveChangesAsync();
            }
        }
    }
}
