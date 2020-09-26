using System;

namespace Szpital.Models
{
    public class Wizyta
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public User Pacjent { get; set; }

        public User Doktor { get; set; }

        public DateTime Data { get; set; }
    }
}
