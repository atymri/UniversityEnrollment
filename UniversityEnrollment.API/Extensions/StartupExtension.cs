using Microsoft.EntityFrameworkCore;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.MappingProfiles;
using UniversityEnrollment.Core.ServiceContracts;
using UniversityEnrollment.Core.Services;
using UniversityEnrollment.Infrastructure.DatabaseContext;
using UniversityEnrollment.Infrastructure.Repositories;

namespace UniversityEnrollment.API.Extensions
{
    public static class StartupExtension
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(CourseMappingProfile).Assembly);
            });

            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();


            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("Default")
                ?? throw new KeyNotFoundException("connection string was not found");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

        }
    }
}
