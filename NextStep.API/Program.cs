using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using NextStep.API.Helper;
using NextStep.Core.Const;
using NextStep.Core.Helper;
using NextStep.Core.Interfaces;
using NextStep.Core.Interfaces.Services;
using NextStep.Core.Models;
using NextStep.Core.Services;
using NextStep.EF.Data;
using NextStep.EF.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
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
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Configure the password constraints
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters = null;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

            // Configure token lifespan
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(7); // Set token lifespan to 7 days
            });


            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"]
                };
            });
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IApplicationHistoryRepository, ApplicationHistoryRepository>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IStepsRepository, StepsRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IApplicationTypeService, ApplicationTypeService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NextStep API", Version = "v1" });

                // Add JWT Authentication to Swagger
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
            });
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
/*                    ApplicationTypeSeed.Initialize(services);
*/
                    // Seed steps
                    StepsSeed.Initialize(services);

                    RequiermentsSeed.Initialize(services);

                    // Seed admin user
                    await SeedAdminUser(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

         
                app.UseSwagger();
                app.UseSwaggerUI();
            

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRole = "ادمن";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminEmail = "Admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "ADMIN",
                    Email = adminEmail
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

    }
       
          public static class StepsSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Steps.Any())
                {
                    return; // DB has been seeded
                }

                var steps = new List<Steps>
                {
                    // ApplicationTypeID 1
                  /*  new Steps { ApplicationTypeID = 1, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 1, DepartmentID = 5, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 1, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 1, DepartmentID = 1, StepOrder = 4 },*/

                    // ApplicationTypeID 2
                    new Steps { ApplicationTypeID = 2, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 2, DepartmentID = 6, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 2, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 2, DepartmentID = 1, StepOrder = 4 },

                    // ApplicationTypeID 3
                    new Steps { ApplicationTypeID = 3, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 3, DepartmentID = 3, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 3, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 3, DepartmentID = 1, StepOrder = 4 },

                    // ApplicationTypeID 4
                    new Steps { ApplicationTypeID = 4, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 4, DepartmentID = 4, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 4, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 4, DepartmentID = 1, StepOrder = 4 },

                    // ApplicationTypeID 5
                    new Steps { ApplicationTypeID = 5, DepartmentID = 5, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 5, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 5, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 6
                    new Steps { ApplicationTypeID = 6, DepartmentID = 6, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 6, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 6, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 7
                    new Steps { ApplicationTypeID = 7, DepartmentID = 3, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 7, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 7, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 8
                    new Steps { ApplicationTypeID = 8, DepartmentID = 4, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 8, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 8, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 9
                    new Steps { ApplicationTypeID = 9, DepartmentID = 5, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 9, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 9, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 10
                    new Steps { ApplicationTypeID = 10, DepartmentID = 6, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 10, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 10, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 11
                    new Steps { ApplicationTypeID = 11, DepartmentID = 3, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 11, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 11, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 12
                    new Steps { ApplicationTypeID = 12, DepartmentID = 4, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 12, DepartmentID = 2, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 12, DepartmentID = 1, StepOrder = 3 },

                    // ApplicationTypeID 13
                    new Steps { ApplicationTypeID = 13, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 13, DepartmentID = 5, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 13, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 13, DepartmentID = 1, StepOrder = 4 },

                    // ApplicationTypeID 14
                    new Steps { ApplicationTypeID = 14, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 14, DepartmentID = 6, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 14, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 14, DepartmentID = 1, StepOrder = 4 },

                    // ApplicationTypeID 15
                    new Steps { ApplicationTypeID = 15, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 15, DepartmentID = 3, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 15, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 15, DepartmentID = 1, StepOrder = 4 },

                    // ApplicationTypeID 16
                    new Steps { ApplicationTypeID = 16, DepartmentID = 7, StepOrder = 1 },
                    new Steps { ApplicationTypeID = 16, DepartmentID = 4, StepOrder = 2 },
                    new Steps { ApplicationTypeID = 16, DepartmentID = 2, StepOrder = 3 },
                    new Steps { ApplicationTypeID = 16, DepartmentID = 1, StepOrder = 4 }
                };

                context.Steps.AddRange(steps);
                context.SaveChanges();
            }
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

    public static class RequiermentsSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Requierments.Any())
                {
                    return; // DB has been seeded
                }

                context.Requierments.AddRange(
                    new Requierments { RequiermentName = "التأكد من نجاح الطالب في جميع المقررات بمعدل لا يقل عن D+." },
                    new Requierments { RequiermentName = "تسجيل موضوع الرسالة في فترة أقصاها فصلين دراسيين." },
                    new Requierments { RequiermentName = "التأكد من حضور الطالب لعدد من السمينارات يعادل 4 ساعات معتمدة وتقديم تقارير عنها." },

                    new Requierments { RequiermentName = "اجتياز سمينار 1." },
                    new Requierments { RequiermentName = "في حالة وجود تعديلات على السمينار الأول، يتم التأكد من إتمام التعديلات." },

                    new Requierments { RequiermentName = "لا تقل نسبة إنجاز الرسالة عن 50%." },
                    new Requierments { RequiermentName = "التأكد من انتهاء فترة القيد." },

                    new Requierments { RequiermentName = "التأكد من نسبة الاقتباس في الرسالة." },
                    new Requierments { RequiermentName = "التأكد من نسبة الاقتباس في البحثين الأول والثاني." },
                    new Requierments { RequiermentName = "وجود الأبحاث المنشورة موقعة من المشرفين." },
                    new Requierments { RequiermentName = "وجود إثبات القبول موقع من المشرفين." },
                    new Requierments { RequiermentName = "إعلان السمينار التأهيلي وتقرير الحضور." },
                    new Requierments { RequiermentName = "حضور حلقتي نقاش (سمينارين)." },
                    new Requierments { RequiermentName = "توفر عدد نسخ الرسالة بالعربية والإنجليزية موقعة من المشرفين." },

                    new Requierments { RequiermentName = "وجود سبب مقبول لإيقاف القيد (مثل: مرض – خدمة عسكرية – رعاية طفل)." },
                    new Requierments { RequiermentName = "استكمال الأوراق اللازمة لتقديم الطلب." },

                    new Requierments { RequiermentName = "قبول الرسالة من قبل المشرفين." },
                    new Requierments { RequiermentName = "وجود مستندات الطالب وتقارير السمينارات." },
                    new Requierments { RequiermentName = "إخلاء طرف من المكتبة المركزية." },

                    new Requierments { RequiermentName = "انقطاع الطالب عن الدراسة." },
                    new Requierments { RequiermentName = "عدم سداد المصروفات." },
                    new Requierments { RequiermentName = "عدم مناقشة الرسالة في المدة المحددة بحسب اللائحة." },
                    new Requierments { RequiermentName = "السفر بدون إذن مسبق وعدم الحصول على منحة رسمية." },
                    new Requierments { RequiermentName = "تكرار الرسوب في المقررات." },
                    new Requierments { RequiermentName = "عدم الجدية في استكمال الرسالة." }
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

            foreach (SystemRoles role in Enum.GetValues(typeof(SystemRoles)))
            {
                // Get the display name attribute value
                var displayAttribute = typeof(SystemRoles)
                    .GetMember(role.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>();

                var roleName = displayAttribute?.Name ?? role.ToString();

                // Check if role exists
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Create the role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
      


    }

   
  