using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly HgsdbContext _context;

        public UserRepository(HgsdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<User>> GetAllUsersWithTeacherAndParentInfoAsync()
        {
            return await _context.Users
                .Include(u => u.Teacher)
                .Include(u => u.Parent)
                .ToListAsync();
        }

        public async Task<User> GetUserWithTeacherAndParentInfoAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Teacher)
                .Include(u => u.Parent)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetByIdForUpdateAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }
        public async Task<int> GetMaxUserIdAsync()
        {
            return await _context.Users.AnyAsync() ? await _context.Users.MaxAsync(u => u.UserId) : 0;
        }
    }
}