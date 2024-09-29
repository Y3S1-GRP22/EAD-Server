using System;
using EAD.Repositories;
using MongoDB.Driver;

namespace EAD.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IMongoCollection<User> _users;

        public UserService(UserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task RegisterAsync(User newUser)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(newUser.Email);
            if (existingUser != null) throw new Exception("User already exists");

            newUser.Password = PasswordHasher.HashPassword(newUser.Password);

            await _userRepository.CreateUserAsync(newUser);
        }


        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.Password))
                throw new Exception("Invalid credentials");

            if (user.IsActive == false)
            {
                throw new Exception("User account is deactivated. Please contact support.");
            }
            return _jwtService.GenerateToken(user.Email, user.Role, user.Username, user.Id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<List<User>> GetAllAdminsAsync()
        {
            return await _userRepository.GetAllAdminsAsync();
        }

        public async Task<List<User>> GetAllVendorsAsync()
        {
            return await _userRepository.GetAllVendorsAsync();
        }

        public async Task<List<User>> GetAllCsrsAsync()
        {
            return await _userRepository.GetAllCsrsAsync();
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            return await _userRepository.ActivateUserAsync(userId);
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            return await _userRepository.DeactivateUserAsync(userId);
        }

    }
}