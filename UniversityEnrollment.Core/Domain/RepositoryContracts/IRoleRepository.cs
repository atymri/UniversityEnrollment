using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;

namespace UniversityEnrollment.Core.Domain.RepositoryContracts
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetRoleWithUsersAsync(Guid roleId);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<List<Role>?> GetUserRolesAsync(Guid userId);
    }
}
