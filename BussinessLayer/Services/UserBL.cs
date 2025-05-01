using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasherBL _passwordHasherBL;
        private readonly IOtpServiceBL _otpServiceBL;
        private readonly ITokenServiceBL _tokenServiceBL;
        private readonly ILogger<UserBL> _logger;

        public UserBL(
            IUserRL userRL,
            IEmailService emailService,
            IPasswordHasherBL passwordHasherBL,
            IOtpServiceBL otpServiceBL,
            ITokenServiceBL tokenServiceBL,
            ILogger<UserBL> logger)
        {
            _userRL = userRL;
            _emailService = emailService;
            _passwordHasherBL = passwordHasherBL;
            _otpServiceBL = otpServiceBL;
            _tokenServiceBL = tokenServiceBL;
            _logger = logger;
        }

        public async Task<UserModel> RegisterUserAsync(RegisterModel model)
        {
            _logger.LogInformation("Registering user: {Email}", model.Email);
            return await RegisterAsync(model, "User");
        }

        public async Task<UserModel> RegisterAdminAsync(RegisterModel model)
        {
            _logger.LogInformation("Registering admin: {Email}", model.Email);
            return await RegisterAsync(model, "Admin");
        }

        private async Task<UserModel> RegisterAsync(RegisterModel model, string role)
        {
            try
            {
                _logger.LogInformation("Processing registration for {Role}: {Email}", role, model.Email);
                var hashedPassword = await _passwordHasherBL.HashPasswordAsync(model.Password);

                var userEntity = new UserEntity
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = hashedPassword,
                    Role = role
                };

                var createdUser = await _userRL.RegisterAsync(userEntity);
                _logger.LogInformation("{Role} registered successfully: {Email}", role, model.Email);
                return MapUserEntityToModel(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Role}: {Email}", role, model.Email);
                throw;
            }
        }

        public async Task<UserModel> LoginAsync(LoginModel model)
        {
            try
            {
                _logger.LogInformation("Login attempt: {Email}", model.Email);
                var user = await _userRL.GetUserByEmailAsync(model.Email);

                if (user == null || !await _passwordHasherBL.VerifyPasswordAsync(model.Password, user.Password))
                {
                    _logger.LogWarning("Login failed: Invalid credentials for {Email}", model.Email);
                    return null;
                }

                user.Token = await _tokenServiceBL.GenerateJwtTokenAsync(user);
                _logger.LogInformation("Login successful: {Email}", model.Email);

                return new UserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Token = user.Token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed: {Email}", model.Email);
                throw;
            }
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            try
            {
                _logger.LogInformation("Forgot password request: {Email}", email);
                var user = await _userRL.GetUserByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("Forgot password failed: User not found {Email}", email);
                    throw new InvalidOperationException("User not found.");
                }

                string otp = await _otpServiceBL.GenerateOtpAsync();
                await _otpServiceBL.StoreOtpAsync(email, otp);

                string emailBody = $"Your password reset OTP: {otp}. This OTP is valid for 10 minutes.";
                await _emailService.SendEmailAsync(email, "Password Reset OTP", emailBody);

                _logger.LogInformation("OTP sent to {Email}", email);
                return "OTP sent to your email.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password process failed: {Email}", email);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
        {
            try
            {
                _logger.LogInformation("Resetting password: {Email}", model.Email);

                if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
                {
                    _logger.LogWarning("Password reset failed: Weak password for {Email}", model.Email);
                    throw new InvalidOperationException("Password must be at least 6 characters");
                }

                if (!await _otpServiceBL.ValidateOtpAsync(model.Email, model.Otp))
                {
                    _logger.LogWarning("Password reset failed: Invalid OTP for {Email}", model.Email);
                    throw new InvalidOperationException("Invalid or expired OTP.");
                }

                var hashedPassword = await _passwordHasherBL.HashPasswordAsync(model.NewPassword);

                var user = await _userRL.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset failed: User not found {Email}", model.Email);
                    throw new InvalidOperationException("User not found.");
                }

                user.Password = hashedPassword;
                await _userRL.UpdatePasswordAsync(user);
                _logger.LogInformation("Password reset successful: {Email}", model.Email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset password: {Email}", model.Email);
                throw;
            }
        }

        private UserModel MapUserEntityToModel(UserEntity user)
        {
            return new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = user.Token
            };
        }
    }
}
