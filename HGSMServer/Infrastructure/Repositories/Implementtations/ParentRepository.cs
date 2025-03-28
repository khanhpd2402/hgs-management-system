using Domain.Models;
//using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ParentRepository : IParentRepository
    {
        private readonly HgsdbContext _context;

        public ParentRepository(HgsdbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Parent parent)
        {
            await _context.Parents.AddAsync(parent);
            await _context.SaveChangesAsync();
        }

        public async Task AddStudentParentAsync(StudentParent studentParent)
        {
            await _context.StudentParents.AddAsync(studentParent);
            await _context.SaveChangesAsync();
        }

        public async Task<StudentParent> GetStudentParentAsync(int studentId, int parentId)
        {
            return await _context.StudentParents
                .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId);
        }

        public async Task<Parent> GetByIdAsync(int parentId)
        {
            return await _context.Parents
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ParentId == parentId);
        }

        public async Task<Parent> GetParentByUserIdAsync(int userId)
        {
            return await _context.Parents
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Parent> GetParentByDetailsAsync(string fullName, DateOnly? dob, string phoneNumber, string email, string idcardNumber)
        {
            return await _context.Parents
                .Include(p => p.User)
                .FirstOrDefaultAsync(p =>
                    p.FullName == fullName &&
                    p.Dob == dob &&
                    p.IdcardNumber == idcardNumber &&
                    p.User.PhoneNumber == phoneNumber &&
                    p.User.Email == email);
        }
    }
}