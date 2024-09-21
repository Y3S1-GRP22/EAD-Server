using System;
using EAD.Repositories;

namespace EAD.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly JwtService _jwtService;

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

            return _jwtService.GenerateToken(user.Email, user.Role);
        }


    }
}