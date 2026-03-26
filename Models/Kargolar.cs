using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiparisTakipSistemi.Models
{
    public class Kargolar
    {
        [Key]
        public int KargoID { get; set; }

        public int KullaniciID { get; set; }  

        public int? KargoFirmaID { get; set; } 

        [Required]
        public string TakipNumarasi { get; set; } = string.Empty;

        public string Durum { get; set; } = string.Empty;

        public string Konum { get; set; } = string.Empty;

        public int? TahminiTeslimGun { get; set; } 

        
        public virtual Kullanicilar Kullanici { get; set; } = null!;

        public virtual KargoFirmalari? KargoFirmasi { get; set; }

        public virtual ICollection<Siparisler> Siparisler { get; set; } = new List<Siparisler>();
    }
}
