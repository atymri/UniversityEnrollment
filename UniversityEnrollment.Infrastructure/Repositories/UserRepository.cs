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
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<User> entities)
        {
            await _context.Users.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate)
            => _context.Users.AnyAsync(predicate);

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public Task<User?> GetByIdAsync(Guid id)
            => _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public Task<User?> GetUserByEmailAsync(string email)
            => _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetUserWithEnrollmentsAsync(Guid id)
            => await _context.Users
                .Include(u => u.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<User>> GetUserByRoleAsync(Guid roleId)
            => await _context.Users.Where(u => u.RoleId == roleId).ToListAsync();

        public async Task<bool> IsEmailDuplicatedAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<User> AddInRole(Guid userId, Guid roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId)
                ?? throw new InvalidOperationException("Role not found.");

            var user = await GetByIdAsync(userId);
            role.Users.Add(user);
            _context.Roles.Update(role);

            user.RoleId = roleId;
            user.Role = role;
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found after adding role.");
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
