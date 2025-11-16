using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Infrastructure.DatabaseContext;

namespace UniversityEnrollment.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Role entity)
        {
            await _context.Roles.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Role> entities)
        {
            await _context.Roles.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role entity)
        {
            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate)
            => _context.Roles.AnyAsync(predicate);

        public async Task<IEnumerable<Role>> GetAllAsync()
            => await _context.Roles.ToListAsync();

        public Task<Role?> GetByIdAsync(Guid id)
            => _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

        public Task<Role?> GetRoleByNameAsync(string roleName)
            => _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        public async Task<Role?> GetRoleWithUsersAsync(Guid roleId)
            => await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == roleId);

        public async Task<List<Role>?> GetUserRolesAsync(Guid userId)
            => await _context.Roles
                .Where(ur => ur.Users.Any(r => r.Id == userId))
                .ToListAsync();

        public async Task UpdateAsync(Role entity)
        {
            _context.Roles.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
