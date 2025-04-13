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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the problematic relationship
            modelBuilder.Entity<ApplicationHistory>()
                .HasOne(ah => ah.Department)
                .WithMany(d => d.ApplicationHistories)
                .HasForeignKey(ah => ah.ActionByDeptId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

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
        }
    }

}
