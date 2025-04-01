using Domain.Models;
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
            // Cập nhật để sử dụng các trường mới của Parent
            return await _context.Parents
                .Include(p => p.User)
                .FirstOrDefaultAsync(p =>
                    (p.FullNameFather == fullName || p.FullNameMother == fullName || p.FullNameGuardian == fullName) &&
                    (p.YearOfBirthFather == dob || p.YearOfBirthMother == dob || p.YearOfBirthGuardian == dob) &&
                    (p.IdcardNumberFather == idcardNumber || p.IdcardNumberMother == idcardNumber || p.IdcardNumberGuardian == idcardNumber) &&
                    (p.PhoneNumberFather == phoneNumber || p.PhoneNumberMother == phoneNumber || p.PhoneNumberGuardian == phoneNumber || p.User.PhoneNumber == phoneNumber) &&
                    (p.EmailFather == email || p.EmailMother == email || p.EmailGuardian == email || p.User.Email == email));
        }

        public async Task UpdateAsync(Parent parent)
        {
            _context.Parents.Update(parent);
            await _context.SaveChangesAsync();
        }
    }
}