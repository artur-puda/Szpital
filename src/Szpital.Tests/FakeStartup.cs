using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Szpital.Database;
using Szpital.Models;
using Szpital.Services;

namespace Szpital.Tests
{
    public class FakeStartup
    {
        public FakeStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddTransient<AccountService>();
            services.AddTransient<WizytyService>();

            services.Configure<Settings>(Configuration.GetSection("CustomSettings"));

            services.AddDbContext<Context>(x => x.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services
            .AddRazorPages(x =>
            {
                x.Conventions.AddPageRoute("/Index", "");
            })
            .AddApplicationPart(typeof(Startup).Assembly)
            .AddRazorRuntimeCompilation();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole(Consts.AdminRoleName));
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, x =>
                {
                    x.LoginPath = "/SignIn";
                    x.AccessDeniedPath = "/SignIn";
                    x.Cookie.Name = "Szpital";
                    x.Cookie.HttpOnly = true;
                    x.Cookie.IsEssential = true;
                    x.ExpireTimeSpan = new TimeSpan(24, 0, 0);
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Context _context)
        {
            _context.Database.EnsureCreated();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
