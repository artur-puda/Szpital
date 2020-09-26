using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Szpital.Models;

namespace Szpital.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wizyta>().Property(x => x.Id).ValueGeneratedNever();

            modelBuilder.Entity<Role>().HasData
            (
                new Role
                {
                    Id = 1,
                    Name = Consts.AdminRoleName
                },
                new Role
                {
                    Id = 2,
                    Name = Consts.PacjentRoleName
                },
                new Role
                {
                    Id = 3,
                    Name = Consts.DoktorRoleName
                }
            );

            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                UserName = "Administrator",
                Email = "admin@szpital.pl",
                RoleId = 1
            };

            var user2 = new User
            {
                UserName = "Urszula Miodek",
                Email = "pacjent@szpital.pl",
                RoleId = 2
            };

            var user3 = new User
            {
                UserName = "Dr. Kowalski Adam",
                Email = "doktor@szpital.pl",
                RoleId = 3
            };

            user.PasswordHash = hasher.HashPassword(user, "123");
            user2.PasswordHash = hasher.HashPassword(user2, "123");
            user3.PasswordHash = hasher.HashPassword(user3, "123");

            modelBuilder.Entity<User>().HasData
            (
                user,
                user2,
                user3
            );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Wizyta> Wizyty { get; set; }
    }
}
