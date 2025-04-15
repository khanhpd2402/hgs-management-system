using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ISubjectRepository
    {

        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject> GetByIdAsync(int id);
        Task<Subject> CreateAsync(Subject entity);
        Task UpdateAsync(Subject entity);
        Task DeleteAsync(int id);
        Task<Subject> GetByNameAsync(string subjectName);

    }

}
