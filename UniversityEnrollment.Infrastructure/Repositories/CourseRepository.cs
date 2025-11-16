using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Infrastructure.DatabaseContext;

namespace UniversityEnrollment.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Course entity)
        {
            await _context.Courses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Course> entities)
        {
            await _context.Courses.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Course entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(Expression<Func<Course, bool>> predicate)
            => _context.Courses.AnyAsync(predicate);

        public async Task<IEnumerable<Course>> GetAllAsync()
            => await _context.Courses.ToListAsync();

        public Task<Course?> GetByIdAsync(Guid id)
            => _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        public Task<Course?> GetCourseByCodeAsync(string courseCode)
            => _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);

        public async Task<List<Course>> GetCoursesWithEnrollmentsAsync()
            => await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.User)
                .ToListAsync();

        public Task<bool> IsCourseCodeDuplicatedAsync(string courseCode)
            => _context.Courses.AnyAsync(c => c.CourseCode == courseCode);

        public async Task UpdateAsync(Course entity)
        {
            _context.Courses.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
