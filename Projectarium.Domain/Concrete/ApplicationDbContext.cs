using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projectarium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
namespace Projectarium.Domain.Concrete
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private readonly string _conStr = "Data Source =(localdb)\\MSSQLLocalDB;Initial Catalog = ProjectariumDb; Integrated Security = True";
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

            Database.EnsureCreated();
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Один-к-одному объеденяющий пользователя и аккаунт идентификации
            modelBuilder
       .Entity<ApplicationUser>()
       .HasOne(u => u.UserProfile)
       .WithOne(p => p.ApplicationUser)
       .HasForeignKey<UserProfile>(p => p.Id)
       .HasPrincipalKey<ApplicationUser>(c => c.Id);

            modelBuilder.Entity<Skill>()
             .HasOne(a => a.Vacancy)
             .WithMany(a => a.Skills)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Skill>()
          .HasOne(a => a.UserProfile)
          .WithMany(a => a.Skills)
          .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
     
          


         
        } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conStr);
        }
    }
}
