using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Szpital.Database;
using Szpital.Models;

namespace Szpital.Pages.Account
{
    [Authorize(Roles = Consts.AdminRoleName)]
    public class UstawRoleModel : PageModel
    {
        private readonly Context _context;

        public UstawRoleModel(Context context)
        {
            _context = context;
        }

        public List<User> Uzytkownicy { get; set; } = new List<User>();

        public async Task OnGet()
        {
            Uzytkownicy = await _context.Users.Include(x => x.Role).Where(x => x.Role.Name == Consts.DoktorRoleName || x.Role.Name == Consts.PacjentRoleName).ToListAsync();
        }

        public async Task<IActionResult> OnPost([FromForm] ChangeRoleInput data)
        {
            if (data != null && data.RoleId != 1)
            {
                var usr = await _context.Users.Include(x => x.Role).FirstAsync(x => x.Id == data.UserId);
                var role = await _context.Roles.FirstAsync(x => x.Id == data.RoleId);
                usr.Role = role;
                usr.RoleId = data.RoleId;

                await _context.SaveChangesAsync();
            }
            return RedirectToPage("UstawRole");
        }
    }
}