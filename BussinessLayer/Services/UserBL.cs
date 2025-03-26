using BusinessLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private static Dictionary<string, (string otp, DateTime expiry)> _otpStorage = new Dictionary<string, (string, DateTime)>();

        public UserBL(IUserRL userRL, IConfiguration configuration, IEmailService emailService)
        {
            _userRL = userRL;
            _configuration = configuration;
            _emailService = emailService;
        }

        public UserEntity Register(RegisterModel model)
        {
            var hashedPassword = HashPassword(model.Password);

            var userEntity = new UserEntity
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = hashedPassword
            };

            return _userRL.Register(userEntity);
        }

        public UserEntity Login(LoginModel model)
        {
            var user = _userRL.GetUserByEmail(model.Email);
            if (user == null) return null;

            if (!VerifyPassword(model.Password, user.Password)) return null;

            user.Token = GenerateJwtToken(user);

            return user;
        }


        public string ForgotPassword(string email)
        {
            var user = _userRL.GetUserByEmail(email);
            if (user == null) throw new InvalidOperationException("User not found.");

            string otp = GenerateOtp();

            _otpStorage[email] = (otp, DateTime.Now.AddMinutes(10));

            string emailBody = $"Your password reset OTP: {otp}. This OTP is valid for 10 minutes.";

            _emailService.SendEmail(email, "Password Reset OTP", emailBody);
            return "OTP sent to your email.";
        }

        public bool ResetPassword(string email, string otp, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new InvalidOperationException("Password must be at least 6 characters");

            if (!_otpStorage.TryGetValue(email, out var otpData))
                throw new InvalidOperationException("OTP not found or expired.");

            if (otpData.otp != otp)
                throw new InvalidOperationException("Invalid OTP.");

            if (DateTime.Now > otpData.expiry)
            {
                _otpStorage.Remove(email);
                throw new InvalidOperationException("OTP has expired.");
            }

            var hashedPassword = HashPassword(newPassword);

            var user = _userRL.GetUserByEmail(email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            _userRL.UpdatePassword(user, hashedPassword);

            _otpStorage.Remove(email);

            return true;
        }

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return string.Equals(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
        new Claim("Id", user.Id.ToString()),
        new Claim("Email", user.Email),
        new Claim("FirstName", user.FirstName),
        new Claim("LastName", user.LastName)
    };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
                signingCredentials: new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}