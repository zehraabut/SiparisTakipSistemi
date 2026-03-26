using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiparisTakipSistemi.Models
{
    public class KargoFirmalari
    {
        [Key]
        public int KargoFirmaID { get; set; }

        [Required(ErrorMessage = "Firma adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Firma adı en fazla 100 karakter olabilir.")]
        public string FirmaAdi { get; set; } = string.Empty;

        
        public ICollection<Kargolar> Kargolar { get; set; } = new List<Kargolar>();
    }
}
