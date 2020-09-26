using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Szpital.Database;
using Szpital.Models;

namespace Szpital.Services
{
    public class WizytyService
    {
        private readonly Context _context;

        public WizytyService(Context context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> UtworzWizyte(User user, WizytaCreate data)
        {
            if (data.Date == default || data.Time == default)
                return (false, "Niepoprawna data wizyty");

            var dt = new DateTime(data.Date.Year, data.Date.Month, data.Date.Day, data.Time.Hours, data.Time.Minutes, 0);

            var wizyta = new Wizyta();
            wizyta.Pacjent = user;
            wizyta.Doktor = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Role.Name == Consts.DoktorRoleName && x.Id == data.DoktorId);

            if (wizyta.Doktor == null)
                return (false, "Nie można odnaleźć doktora");

            var juzZajete = await _context.Wizyty.AnyAsync(x => x.Data == dt);

            if (juzZajete)
            {
                return (false, "Podana data jest już zarezerwowana");
            }

            wizyta.Data = dt;

            await _context.AddAsync(wizyta);

            await _context.SaveChangesAsync();

            return (true, "");
        }

        public async Task<List<Wizyta>> PobierzWizytyPoDacie(User user, DateTime dt)
        {
            return await _context.Wizyty.Include(x => x.Pacjent).Include(x => x.Doktor)
                .Where(x => x.Pacjent.Id == user.Id || x.Doktor.Id == user.Id)
                .Where(x => x.Data > dt)
                .ToListAsync();
        }
    }
}
