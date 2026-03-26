using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SiparisTakipSistemi.Models
{
    public partial class Kullanicilar
    {
        public int KullaniciID { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = null!;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Eposta { get; set; } = null!;

        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Kullanıcı";  // Varsayılan olarak kullanıcı rolü atanıyor

        public virtual ICollection<Siparisler> Siparislers { get; } = new List<Siparisler>();
    }
}
