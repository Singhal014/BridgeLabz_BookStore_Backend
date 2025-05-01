using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRL> _logger;

        public UserRL(ApplicationDbContext context, IConfiguration configuration, ILogger<UserRL> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<UserEntity> RegisterAsync(UserEntity user)
        {
            try
            {
                _logger.LogInformation("Checking if Email is already registered: {Email}", user.Email);

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration denied. Email already registered: {Email}", user.Email);
                    throw new InvalidOperationException("Email is already registered.");
                }

                _logger.LogInformation("Registering new user with Email: {Email}", user.Email);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RepoLayer.UserRL.RegisterAsync");
                throw;
            }
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Fetching user details for Email: {Email}", email);
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RepoLayer.UserRL.GetUserByEmailAsync");
                throw;
            }
        }

        public async Task<UserEntity> GetUserByIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching user details for User ID: {UserId}", userId);
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RepoLayer.UserRL.GetUserByIdAsync");
                throw;
            }
        }

        public async Task UpdatePasswordAsync(UserEntity user)
        {
            try
            {
                _logger.LogInformation("Updating password for User ID: {UserId}", user.Id);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Password updated successfully for User ID: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RepoLayer.UserRL.UpdatePasswordAsync");
                throw;
            }
        }
    }
}
