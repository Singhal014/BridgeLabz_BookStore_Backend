using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System.Linq;

namespace RepoLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRL(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public UserEntity Register(UserEntity user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public UserEntity Login(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public UserEntity GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public void UpdatePassword(UserEntity user, string newPassword)
        {
            user.Password = newPassword;
            _context.SaveChanges();
        }
    }
}