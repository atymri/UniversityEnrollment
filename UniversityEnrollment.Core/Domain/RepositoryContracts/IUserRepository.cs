using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;

namespace UniversityEnrollment.Core.Domain.RepositoryContracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserWithEnrollmentsAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetUserByRoleAsync(Guid roleId);
        Task<bool> IsEmailDuplicatedAsync(string email);
        Task<User> AddInRole(Guid userId, Guid roleId);
    }
}
