using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Szpital.Database;
using Szpital.Models;
using Szpital.Services;

namespace Szpital.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly WizytyService _service;
        private readonly Context _context;

        public IndexModel(WizytyService service, Context context)
        {
            _service = service;
            _context = context;
        }

        public List<Wizyta> Wizyty { get; set; }

        public async Task OnGet()
        {
            var email = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstAsync(x => x.Email == email);
            Wizyty = await _service.PobierzWizytyPoDacie(user, DateTime.Now);
        }
    }
}