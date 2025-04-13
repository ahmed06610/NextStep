
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using NextStep.Core.Const;
using NextStep.Core.Models;
using NextStep.EF.Data;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;


namespace NextStep.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Seed departments
                    DepartmentSeed.Initialize(services);

                    // Seed roles
                    await RoleSeed.Initialize(services);
                    // Seed application types
                    ApplicationTypeSeed.Initialize(services);

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
        
    }
    public static class DepartmentSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Departments.Any())
                {
                    return; // DB has been seeded
                }

                context.Departments.AddRange(
                    new Department { DepartmentName = "مجلس الكليه" },
                    new Department { DepartmentName = "لجنه الدرسات العليا" },
                    new Department { DepartmentName = "حسابات علميه" },
                    new Department { DepartmentName = "ذكاء اصطناعي" },
                    new Department { DepartmentName = "علوم حاسب" },
                    new Department { DepartmentName = "نظم المعلومات" },
                    new Department { DepartmentName = "إدارة الدرسات العليا" }
                );

                context.SaveChanges();
            }
        }
    }

    public static class RoleSeed
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // List of department-based roles
            string[] departmentRoles = {
                    " موظف مجلس الكليه",
                    " موظف لجنه الدرسات العليا",
                    " موظف حسابات علميه",
                    " موظق ذكاء اصطناعي",
                    " موظف علوم حاسب",
                    " موظف نظم المعلومات",
                    " موظف إدارة الدرسات العليا",
                    "طالب",
                };

            foreach (var roleName in departmentRoles)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }

    public static class ApplicationTypeSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.ApplicationTypes.Any())
                {
                    return; // DB has been seeded
                }

                var applicationTypes = Enum.GetValues(typeof(ApplicationTypeEnum))
                    .Cast<ApplicationTypeEnum>()
                    .Select(e => new ApplicationType
                    {
                        ApplicationTypeName = e.GetDisplayName(),
                        Description = e.GetDisplayDescription()
                    });

                context.ApplicationTypes.AddRange(applicationTypes);
                context.SaveChanges();
            }
        }

        // Helper extension methods to get display attributes
        private static string GetDisplayName(this Enum value)
        {
            return value.GetType()
                       .GetMember(value.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()?
                       .Name ?? value.ToString();
        }

        private static string GetDisplayDescription(this Enum value)
        {
            return value.GetType()
                       .GetMember(value.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()?
                       .GetDescription() ?? string.Empty;
        }
    }
}
