using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Szpital.Database;
using Szpital.Models;
using Szpital.Models.Input;
using Szpital.Services;
using Xunit;

namespace Szpital.Tests
{
    public class TestyKont
    {
        [Fact]
        public async Task Da_sie_utworzyc_uzytkownika()
        {
            var context = GetFakeDatabaseContext();
            var service = new AccountService(context, new PasswordHasher<User>());

            var input = new AccountInfo
            {
                Email = "test@localhost",
                Password = "123345",
                UserName = "test5"
            };

            var result = await service.TryRegister(input);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task Nie_da_sie_utworzyc_2_uzytkownikow_na_tego_samego_emaila()
        {
            var context = GetFakeDatabaseContext();
            var service = new AccountService(context, new PasswordHasher<User>());

            var input = new AccountInfo
            {
                Email = "test@localhost",
                Password = "123345",
                UserName = "test5"
            };

            var result = await service.TryRegister(input);
            Assert.True(result.Success);

            input = new AccountInfo
            {
                Email = "test@localhost",
                Password = "5",
                UserName = "5"
            };

            result = await service.TryRegister(input);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Nie_da_sie_utworzyc_uzytkownika_bez_emaila()
        {
            var context = GetFakeDatabaseContext();
            var service = new AccountService(context, new PasswordHasher<User>());

            var input = new AccountInfo
            {
                Email = "",
                Password = "123345",
                UserName = "test5"
            };

            var result = await service.TryRegister(input);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Nie_da_sie_utworzyc_uzytkownika_bez_hasla()
        {
            var context = GetFakeDatabaseContext();
            var service = new AccountService(context, new PasswordHasher<User>());

            var input = new AccountInfo
            {
                Email = "test@localhost",
                Password = "",
                UserName = "234"
            };

            var result = await service.TryRegister(input);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Nie_da_sie_utworzyc_uzytkownika_bez_nazwy()
        {
            var context = GetFakeDatabaseContext();
            var service = new AccountService(context, new PasswordHasher<User>());

            var input = new AccountInfo
            {
                Email = "test@localhost",
                Password = "dfg34",
                UserName = ""
            };

            var result = await service.TryRegister(input);
            Assert.False(result.Success);
        }

        private Context GetFakeDatabaseContext()
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString("N"));
            var context = new Context(options.Options);
            SeedDatabase(context);
            return context;
        }

        private void SeedDatabase(Context context)
        {
            context.Roles.Add(new Role
            {
                Id = 1,
                Name = Consts.AdminRoleName
            });

            context.Roles.Add(new Role
            {
                Id = 2,
                Name = Consts.DoktorRoleName
            });

            context.Roles.Add(new Role
            {
                Id = 3,
                Name = Consts.PacjentRoleName
            });

            context.SaveChanges();
        }
    }
}
