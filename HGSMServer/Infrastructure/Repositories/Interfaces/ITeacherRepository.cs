﻿using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        IQueryable<Teacher> GetAll();
        Task<Teacher?> GetByIdAsync(int id);
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string idCard, string insuranceNumber); // Kiểm tra giáo viên có tồn tại không
        Task AddRangeAsync(IEnumerable<Teacher> teachers); // Thêm danh sách giáo viên
        Task<Teacher> GetByUserIdAsync(int userId);
    }
}
