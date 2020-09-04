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
        private readonly string _conStr = "Data Source =.\\SQLEXPRESS;Initial Catalog = ProjectariumDb; Integrated Security = True";
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

            //Database.EnsureCreated();
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
       .HasPrincipalKey<ApplicationUser>(c => c.Id)
   ;
            //Многие-ко-многим объеденяющее умения и пользователей 
            modelBuilder.Entity<SkillUser>()
         .HasKey(t => new { t.SkillId, t.UserId });

            modelBuilder.Entity<SkillUser>()
           .HasOne(sc => sc.Skill)
           .WithMany(s => s.SkillUsers)
           .HasForeignKey(sc => sc.SkillId);

            modelBuilder.Entity<SkillUser>()
                .HasOne(sc => sc.UserProfile)
                .WithMany(c => c.SkillUsers)
                .HasForeignKey(sc => sc.UserId);
          

            //Многие-ко-многим объеденяющее умения и вакансии
            modelBuilder.Entity<SkillVacancy>()
       .HasKey(t => new { t.SkillId, t.VacancyId });

            modelBuilder.Entity<SkillVacancy>()
           .HasOne(sc => sc.Skill)
           .WithMany(s => s.SkillVacancies)
           .HasForeignKey(sc => sc.SkillId);

            modelBuilder.Entity<SkillVacancy>()
                .HasOne(sc => sc.Vacancy)
                .WithMany(c => c.SkillVacancies)
                .HasForeignKey(sc => sc.VacancyId);


            base.OnModelCreating(modelBuilder);
     
          


         
        } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conStr);
        }
    }
}
