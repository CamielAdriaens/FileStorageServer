using FileStorage.Models;
using FileStorage.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Get or create a user by their Google ID
        public async Task<User> GetOrCreateUserByGoogleIdAsync(string googleId, string email, string name)
        {
            // Get the user by Google ID
            var user = await _userRepository.GetUserByGoogleId(googleId);
            if (user == null)
            {
                // Create a new user if not found
                user = new User
                {
                    GoogleId = googleId,
                    Email = email,
                    Name = name
                };
                user = await _userRepository.CreateUser(user);
            }
            return user;
        }

        // Get the user's files by their Google ID
        public async Task<List<UserFile>> GetUserFilesAsync(string googleId)
        {
            return await _userRepository.GetUserFiles(googleId);
        }

        // Add a file for a user
        public async Task AddUserFileAsync(string googleId, string mongoFileId, string fileName)
        {
            // Find the user
            var user = await _userRepository.GetUserByGoogleId(googleId);
            if (user != null)
            {
                // Create the file metadata
                var userFile = new UserFile
                {
                    MongoFileId = mongoFileId,
                    FileName = fileName,
                    UploadDate = System.DateTime.UtcNow,
                    UserId = user.Id
                };

                // Add the file metadata to the database
                await _userRepository.AddUserFile(userFile);
            }
        }
    }
}
