using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Szpital.Models.Input;
using Szpital.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Szpital.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AccountService _service;

        public RegisterModel(AccountService _service)
        {
            this._service = _service;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(AccountInfo info)
        {
            if (info == null || string.IsNullOrEmpty(info.Email))
            {
                TempData["ErrorMessage"] = "Dane są uszkodzone.";
                return RedirectToPage("Register");
            }

            var result = await _service.TryRegister(info);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToPage("Register");
            }

            var user = result.User;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.Name),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            await HttpContext.SignInAsync
            (
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return LocalRedirect("/");
        }
    }
}