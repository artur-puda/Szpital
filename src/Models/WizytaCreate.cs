using System;

namespace Szpital.Models
{
    public class WizytaCreate
    {
        public Guid DoktorId { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }
    }
}
