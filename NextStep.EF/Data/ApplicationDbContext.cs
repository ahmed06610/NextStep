using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextStep.Core.Models;
namespace NextStep.EF.Data
{
    

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Identity Users
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // Custom Entities
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationHistory> ApplicationHistories { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<Steps> Steps { get; set; }
        public DbSet<Requierments> Requierments { get; set; }
        public DbSet<RequiermentsApplicationType> RequiermentsApplicationTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the problematic relationship
            modelBuilder.Entity<ApplicationHistory>()
                .HasOne(ah => ah.Department)
                .WithMany(d => d.ApplicationHistories)
                .HasForeignKey(ah => ah.ActionByDeptId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

            modelBuilder.Entity<ApplicationType>()
             .HasOne(at => at.Department)
             .WithMany(d => d.ApplicationTypes)
             .HasForeignKey(at => at.CreatedByDeptId)
             .OnDelete(DeleteBehavior.SetNull); // أو Restrict أو NoAction حسب ما يناسبك


            // Employee has one ApplicationUser
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student has one ApplicationUser
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Application.CreatedBy links to ApplicationUser
            modelBuilder.Entity<Application>()
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Steps relationship
            modelBuilder.Entity<Steps>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Steps)
                .HasForeignKey(s => s.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequiermentsApplicationType>().HasData(
                // 1) الالتحاق (Ids 1-4)
                new { Id = 1, ApplicationTypeId = 1, RequiermentId = 1 },
                new { Id = 2, ApplicationTypeId = 1, RequiermentId = 2 },
                new { Id = 3, ApplicationTypeId = 1, RequiermentId = 3 },
                new { Id = 4, ApplicationTypeId = 1, RequiermentId = 4 },

                new { Id = 5, ApplicationTypeId = 2, RequiermentId = 1 },
                new { Id = 6, ApplicationTypeId = 2, RequiermentId = 2 },
                new { Id = 7, ApplicationTypeId = 2, RequiermentId = 3 },
                new { Id = 8, ApplicationTypeId = 2, RequiermentId = 4 },

                new { Id = 9, ApplicationTypeId = 3, RequiermentId = 1 },
                new { Id = 10, ApplicationTypeId = 3, RequiermentId = 2 },
                new { Id = 11, ApplicationTypeId = 3, RequiermentId = 3 },
                new { Id = 12, ApplicationTypeId = 3, RequiermentId = 4 },

                new { Id = 13, ApplicationTypeId = 4, RequiermentId = 1 },
                new { Id = 14, ApplicationTypeId = 4, RequiermentId = 2 },
                new { Id = 15, ApplicationTypeId = 4, RequiermentId = 3 },
                new { Id = 16, ApplicationTypeId = 4, RequiermentId = 4 },

                // 2) سيمنار 1 تعيين لجنة الاشراف والخطة (Ids 5-7)
                new { Id = 17, ApplicationTypeId = 17, RequiermentId = 5 },
                new { Id = 18, ApplicationTypeId = 17, RequiermentId = 6 },
                new { Id = 19, ApplicationTypeId = 17, RequiermentId = 7 },

                new { Id = 20, ApplicationTypeId = 18, RequiermentId = 5 },
                new { Id = 21, ApplicationTypeId = 18, RequiermentId = 6 },
                new { Id = 22, ApplicationTypeId = 18, RequiermentId = 7 },

                new { Id = 23, ApplicationTypeId = 19, RequiermentId = 5 },
                new { Id = 24, ApplicationTypeId = 19, RequiermentId = 6 },
                new { Id = 25, ApplicationTypeId = 19, RequiermentId = 7 },

                new { Id = 26, ApplicationTypeId = 20, RequiermentId = 5 },
                new { Id = 27, ApplicationTypeId = 20, RequiermentId = 6 },
                new { Id = 28, ApplicationTypeId = 20, RequiermentId = 7 },

                // 3) سيمنار 2 – (Ids 8-9)
                new { Id = 29, ApplicationTypeId = 21, RequiermentId = 8 },
                new { Id = 30, ApplicationTypeId = 21, RequiermentId = 9 },
                new { Id = 31, ApplicationTypeId = 22, RequiermentId = 8 },
                new { Id = 32, ApplicationTypeId = 22, RequiermentId = 9 },
                new { Id = 33, ApplicationTypeId = 23, RequiermentId = 8 },
                new { Id = 34, ApplicationTypeId = 23, RequiermentId = 9 },
                new { Id = 35, ApplicationTypeId = 24, RequiermentId = 8 },
                new { Id = 36, ApplicationTypeId = 24, RequiermentId = 9 },

                // 4) طلب الم (Ids 10-11)
                new { Id = 37, ApplicationTypeId = 5, RequiermentId = 10 },
                new { Id = 38, ApplicationTypeId = 5, RequiermentId = 11 },
                new { Id = 39, ApplicationTypeId = 6, RequiermentId = 10 },
                new { Id = 40, ApplicationTypeId = 6, RequiermentId = 11 },
                new { Id = 41, ApplicationTypeId = 7, RequiermentId = 10 },
                new { Id = 42, ApplicationTypeId = 7, RequiermentId = 11 },
                new { Id = 43, ApplicationTypeId = 8, RequiermentId = 10 },
                new { Id = 44, ApplicationTypeId = 8, RequiermentId = 11 },

                // 5) تشكيل لجنة الحكم و (Ids 12-18)
                new { Id = 45, ApplicationTypeId = 25, RequiermentId = 12 },
                new { Id = 46, ApplicationTypeId = 25, RequiermentId = 13 },
                new { Id = 47, ApplicationTypeId = 25, RequiermentId = 14 },
                new { Id = 48, ApplicationTypeId = 25, RequiermentId = 15 },
                new { Id = 49, ApplicationTypeId = 25, RequiermentId = 16 },
                new { Id = 50, ApplicationTypeId = 25, RequiermentId = 17 },
                new { Id = 51, ApplicationTypeId = 25, RequiermentId = 18 },

                new { Id = 52, ApplicationTypeId = 26, RequiermentId = 12 },
                new { Id = 53, ApplicationTypeId = 26, RequiermentId = 13 },
                new { Id = 54, ApplicationTypeId = 26, RequiermentId = 14 },
                new { Id = 55, ApplicationTypeId = 26, RequiermentId = 15 },
                new { Id = 56, ApplicationTypeId = 26, RequiermentId = 16 },
                new { Id = 57, ApplicationTypeId = 26, RequiermentId = 17 },
                new { Id = 58, ApplicationTypeId = 26, RequiermentId = 18 },

                new { Id = 59, ApplicationTypeId = 27, RequiermentId = 12 },
                new { Id = 60, ApplicationTypeId = 27, RequiermentId = 13 },
                new { Id = 61, ApplicationTypeId = 27, RequiermentId = 14 },
                new { Id = 62, ApplicationTypeId = 27, RequiermentId = 15 },
                new { Id = 63, ApplicationTypeId = 27, RequiermentId = 16 },
                new { Id = 64, ApplicationTypeId = 27, RequiermentId = 17 },
                new { Id = 65, ApplicationTypeId = 27, RequiermentId = 18 },

                new { Id = 66, ApplicationTypeId = 28, RequiermentId = 12 },
                new { Id = 67, ApplicationTypeId = 28, RequiermentId = 13 },
                new { Id = 68, ApplicationTypeId = 28, RequiermentId = 14 },
                new { Id = 69, ApplicationTypeId = 28, RequiermentId = 15 },
                new { Id = 70, ApplicationTypeId = 28, RequiermentId = 16 },
                new { Id = 71, ApplicationTypeId = 28, RequiermentId = 17 },
                new { Id = 72, ApplicationTypeId = 28, RequiermentId = 18 },

                // 6) ط (Ids 19-20)
                new { Id = 73, ApplicationTypeId = 9, RequiermentId = 19 },
                new { Id = 74, ApplicationTypeId = 9, RequiermentId = 20 },

                // 7) ط
                new { Id = 75, ApplicationTypeId = 33, RequiermentId = 21 },
                new { Id = 76, ApplicationTypeId = 33, RequiermentId = 22 },
                new { Id = 77, ApplicationTypeId = 33, RequiermentId = 23 },

                // 8) إلغاء
                new { Id = 78, ApplicationTypeId = 13, RequiermentId = 24 },
                new { Id = 79, ApplicationTypeId = 13, RequiermentId = 25 },
                new { Id = 80, ApplicationTypeId = 13, RequiermentId = 26 },
                new { Id = 81, ApplicationTypeId = 13, RequiermentId = 27 },
                new { Id = 82, ApplicationTypeId = 13, RequiermentId = 28 },
                new { Id = 83, ApplicationTypeId = 13, RequiermentId = 29 },

                new { Id = 84, ApplicationTypeId = 14, RequiermentId = 24 },
                new { Id = 85, ApplicationTypeId = 14, RequiermentId = 25 },
                new { Id = 86, ApplicationTypeId = 14, RequiermentId = 26 },
                new { Id = 87, ApplicationTypeId = 14, RequiermentId = 27 },
                new { Id = 88, ApplicationTypeId = 14, RequiermentId = 28 },
                new { Id = 89, ApplicationTypeId = 14, RequiermentId = 29 },

                new { Id = 90, ApplicationTypeId = 15, RequiermentId = 24 },
                new { Id = 91, ApplicationTypeId = 15, RequiermentId = 25 },
                new { Id = 92, ApplicationTypeId = 15, RequiermentId = 26 },
                new { Id = 93, ApplicationTypeId = 15, RequiermentId = 27 },
                new { Id = 94, ApplicationTypeId = 15, RequiermentId = 28 },
                new { Id = 95, ApplicationTypeId = 15, RequiermentId = 29 },

                new { Id = 96, ApplicationTypeId = 16, RequiermentId = 24 },
                new { Id = 97, ApplicationTypeId = 16, RequiermentId = 25 },
                new { Id = 98, ApplicationTypeId = 16, RequiermentId = 26 },
                new { Id = 99, ApplicationTypeId = 16, RequiermentId = 27 },
                new { Id = 100, ApplicationTypeId = 16, RequiermentId = 28 },
                new { Id = 101, ApplicationTypeId = 16, RequiermentId = 29 },

                //////////////////////////////////////////////////
                ///

                // 8) إلغاء
                new { Id = 102, ApplicationTypeId = 39, RequiermentId = 24 },
                new { Id = 103, ApplicationTypeId = 39, RequiermentId = 25 },
                new { Id = 104, ApplicationTypeId = 39, RequiermentId = 26 },
                new { Id = 105, ApplicationTypeId = 39, RequiermentId = 27 },
                new { Id = 106, ApplicationTypeId = 39, RequiermentId = 28 },
                new { Id = 107, ApplicationTypeId = 39, RequiermentId = 29 },

                new { Id = 108, ApplicationTypeId = 40, RequiermentId = 24 },
                new { Id = 109, ApplicationTypeId = 40, RequiermentId = 25 },
                new { Id = 110, ApplicationTypeId = 40, RequiermentId = 26 },
                new { Id = 111, ApplicationTypeId = 40, RequiermentId = 27 },
                new { Id = 112, ApplicationTypeId = 40, RequiermentId = 28 },
                new { Id = 113, ApplicationTypeId = 40, RequiermentId = 29 },

                new { Id = 114, ApplicationTypeId = 41, RequiermentId = 24 },
                new { Id = 115, ApplicationTypeId = 41, RequiermentId = 25 },
                new { Id = 116, ApplicationTypeId = 41, RequiermentId = 26 },
                new { Id = 117, ApplicationTypeId = 41, RequiermentId = 27 },
                new { Id = 118, ApplicationTypeId = 41, RequiermentId = 28 },
                new { Id = 119, ApplicationTypeId = 41, RequiermentId = 29 },

                new { Id = 120, ApplicationTypeId = 42, RequiermentId = 24 },
                new { Id = 121, ApplicationTypeId = 42, RequiermentId = 25 },
                new { Id = 122, ApplicationTypeId = 42, RequiermentId = 26 },
                new { Id = 123, ApplicationTypeId = 42, RequiermentId = 27 },
                new { Id = 124, ApplicationTypeId = 42, RequiermentId = 28 },
                new { Id = 125, ApplicationTypeId = 42, RequiermentId = 29 }

            // سيمنار المناقشة ليس له متطلبات
            );
        }
    }

}
