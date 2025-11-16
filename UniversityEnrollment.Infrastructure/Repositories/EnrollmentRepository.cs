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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Enrollment entity)
        {
            await _context.Enrollments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Enrollment> entities)
        {
            await _context.Enrollments.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Enrollment entity)
        {
            _context.Enrollments.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(Expression<Func<Enrollment, bool>> predicate)
            => _context.Enrollments.AnyAsync(predicate);

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
            => await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .ToListAsync();

        public async Task<Enrollment?> GetByIdAsync(Guid id)
            => await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<List<Enrollment>?> GetEnrollmentsByUserAsync(Guid userId)
            => await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .ToListAsync();

        public async Task<List<Enrollment>?> GetEnrollmentsByCourseAsync(Guid courseId)
            => await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.User)
                .ToListAsync();

        public async Task<int> GetTotalUnitsForUserAsync(Guid userId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .ToListAsync();

            return enrollments.Sum(e => e.Course?.Units ?? 0);
        }

        public async Task<bool> IsUserAlreadyEnrolledAsync(Guid userId, Guid courseId)
            => await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

        public async Task UpdateAsync(Enrollment entity)
        {
            _context.Enrollments.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
