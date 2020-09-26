using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Szpital.Database;
using Szpital.Models;
using Szpital.Services;

namespace Szpital.Pages.Wizyty
{
    public class CreateModel : PageModel
    {
        private readonly Context _context;

        private readonly WizytyService _service;

        public CreateModel(Context context, WizytyService service)
        {
            _context = context;
            _service = service;
        }

        public List<User> Doktorzy { get; set; }

        public string Error { get; set; }

        public async Task OnGet()
        {
            Error = TempData["ErrorMessage"]?.ToString() ?? "";
            Doktorzy = await _context.Users.Include(x => x.Role).Where(x => x.Role.Name == Consts.DoktorRoleName).ToListAsync();
        }

        public async Task<IActionResult> OnPost([FromForm] WizytaCreate data)
        {
            if (data != null)
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.Users.FirstAsync(x => x.Email == email);
                var result = await _service.UtworzWizyte(user, data);

                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }

            return Redirect("/Index");
        }
    }
}