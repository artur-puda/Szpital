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
    public class SignInModel : PageModel
    {
        private readonly AccountService _service;

        public SignInModel(AccountService _service)
        {
            this._service = _service;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnGetLogout()
        {
            await this.HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public async Task<IActionResult> OnPost(AccountInfo info)
        {
            if (info == null || string.IsNullOrEmpty(info.Email))
            {
                TempData["ErrorMessage"] = "Niepoprawne dane logowania.";
                return RedirectToPage("SignIn");
            }

            var user = await _service.GetUserOrNull(info);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Niepoprawne dane logowania.";
                return RedirectToPage("SignIn");
            }

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