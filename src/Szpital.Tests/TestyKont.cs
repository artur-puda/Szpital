using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Szpital.Database;
using Szpital.Models;
using Szpital.Models.Input;
using Szpital.Services;
using Xunit;

namespace Szpital.Tests
{
    public class TestyKont : IClassFixture<CustomWebApplicationFactory<FakeStartup>>
    {
         private readonly CustomWebApplicationFactory<FakeStartup> _factory;

        public TestyKont(CustomWebApplicationFactory<FakeStartup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Register")]
        [InlineData("/SignIn")]
        public async Task test_integracyjny_czy_strony_zwracaja_tresc(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task test_integracyjny_tworzenia_uzytkownika()
        {
            // Arrange
            var client = _factory.CreateClient();

            var csrf = await client.GetAsync("/Register");
            var content = await csrf.Content.ReadAsStringAsync();

            var start = "type=\"hidden\" value=\"";
            var indexOfStart = content.IndexOf(start) + start.Length;
            var indexOfEnd = content.IndexOf("\"", indexOfStart);

            var token = content.Substring(indexOfStart, indexOfEnd - indexOfStart);

            // Act
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "test@localhost"),
                new KeyValuePair<string, string>("Password", "12345"),
                new KeyValuePair<string, string>("UserName", "tes5"),
                new KeyValuePair<string, string>("__RequestVerificationToken", token),
            });

            var response = await client.PostAsync("/Register", formContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

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
