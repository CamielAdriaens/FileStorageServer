﻿using FileStorage.Models;
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

        public async Task<User> GetOrCreateUserByGoogleIdAsync(string googleId, string email, string name)
        {
            try
            {
                // Check if the user already exists in the database
                var user = await _userRepository.GetUserByGoogleId(googleId);
                if (user == null)
                {
                    Console.WriteLine($"Creating new user with Google ID: {googleId}");

                    // Create a new user
                    user = new User
                    {
                        GoogleId = googleId,
                        Email = email,
                        Name = name
                    };

                    user = await _userRepository.CreateUser(user);
                    Console.WriteLine($"User {googleId} created successfully.");
                }
                else
                {
                    Console.WriteLine($"User {googleId} already exists.");
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrCreateUserByGoogleIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<UserFile>> GetUserFilesAsync(string googleId)
        {
            return await _userRepository.GetUserFiles(googleId);
        }

        public async Task AddUserFileAsync(string googleId, string mongoFileId, string fileName)
        {
            var user = await _userRepository.GetUserByGoogleId(googleId);
            if (user != null)
            {
                var userFile = new UserFile
                {
                    MongoFileId = mongoFileId,
                    FileName = fileName,
                    UploadDate = System.DateTime.UtcNow,
                    UserId = user.Id
                };

                await _userRepository.AddUserFile(userFile);
            }
        }

        public async Task RemoveUserFileAsync(string googleId, string mongoFileId)
        {
            var user = await _userRepository.GetUserByGoogleId(googleId);
            if (user != null)
            {
                await _userRepository.RemoveUserFile(user.Id, mongoFileId);
            }
        }
    }
}
