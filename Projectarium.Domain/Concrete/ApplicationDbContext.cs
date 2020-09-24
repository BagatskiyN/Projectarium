using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projectarium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

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
        {     //Один-к-одному объеденяющий пользователя и аккаунт идентификации
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

            modelBuilder.Entity<Vacancy>()
              .HasOne(a => a.Project)
           .WithMany(a => a.Vacancies)
             .HasForeignKey(x => x.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Link>()
          .HasOne(a => a.Project)
          .WithMany(a => a.Links)
             .HasForeignKey(x => x.ProjectId)
          .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Request>()
            .HasOne(a => a.UserProfile)
            .WithMany(a => a.Requests)
             .HasForeignKey(x => x.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Skill>()
          .HasOne(a => a.UserProfile)
          .WithMany(a => a.Skills)
           .HasForeignKey(x => x.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Link>()
        .HasOne(a => a.UserProfile)
        .WithMany(a => a.Links)
           .HasForeignKey(x => x.UserProfileId)
        .OnDelete(DeleteBehavior.Cascade);
        

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conStr);
        }
        public class AdminInitializer
        {
         
       
            public static async Task InitializeAsync(UserManager<ApplicationUser> userManager/*,ApplicationDbContext _context*/)
            {
                string adminEmail = "admin@gmail.com";
                string password = "Admin1@";
                var user = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    ApplicationUser applicationUser = await userManager.FindByEmailAsync(adminEmail);


                    await userManager.AddClaimAsync(user, new Claim("AdminId", applicationUser.Id.ToString()));

                }
                string firstUserEmail = "firstUser@gmail.com";
                string firstUserPassword = "firstUser1";
                var firstUser = new ApplicationUser { UserName = firstUserEmail, Email = firstUserEmail, EmailConfirmed = true };
                var firstUserCreateResult = await userManager.CreateAsync(firstUser, firstUserPassword);
                if (firstUserCreateResult.Succeeded)
                {
                    ApplicationUser firstApplicationUser= await userManager.FindByEmailAsync(firstUserEmail);
                    //UserProfile firstUserProfile = new UserProfile()
                    //{
                    //    Id = firstApplicationUser.Id,
                    //    ApplicationUser = firstApplicationUser,
                    //    Name = "FirstUser",
                    //    AboutUser = "AboutFirstUser"
                    //};
                    //_context.UserProfiles.Add(firstUserProfile);
                    //Project project = new Project()
                    //{
                    //    Title = "Project1",
                    //    AboutProject = "AboutProject1",
                    //    UserProfile =firstUserProfile

                    //};
                    //await _context.SaveChangesAsync();

                    await userManager.AddClaimAsync(user, new Claim("UserId", firstApplicationUser.Id.ToString()));

                }
                string secondUserEmail = "secondUser@gmail.com";
                string secondUserPassword = "secondUser1";
                var secondUser = new ApplicationUser { UserName = secondUserEmail, Email = secondUserEmail, EmailConfirmed = true };
                var secondUserCreateResult = await userManager.CreateAsync(secondUser, secondUserPassword);
                if (secondUserCreateResult.Succeeded)
                {
                    ApplicationUser secondApplicationUser = await userManager.FindByEmailAsync(secondUserEmail);


                    await userManager.AddClaimAsync(user, new Claim("UserId", secondApplicationUser.Id.ToString()));

                }

            

            }
        }

    }
}
