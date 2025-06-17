using AuthNet.Data;
using AuthNet.Models.Domain;

namespace AuthNet.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public bool Register(User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return false;

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public User Authenticate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }
    }
}
