using System.ComponentModel.DataAnnotations;

namespace SiparisTakipSistemi.Models
{
    public class KargoEkleViewModel
    {
        // Kargo bilgileri
        [Required(ErrorMessage = "Kullanıcı seçilmelidir.")]
        public int KullaniciID { get; set; }

        [Required(ErrorMessage = "Kargo firması seçilmelidir.")]
        public int? KargoFirmaID { get; set; }

        [Required(ErrorMessage = "Takip numarası girilmelidir.")]
        [Display(Name = "Takip Numarası")]
        public string TakipNumarasi { get; set; } = string.Empty;

        [Display(Name = "Durum")]
        public string Durum { get; set; } = string.Empty;

        [Display(Name = "Konum")]
        public string Konum { get; set; } = string.Empty;

        [Display(Name = "Tahmini Teslim Günü")]
        [Range(1, 30, ErrorMessage = "Teslim günü 1 ile 30 arasında olmalıdır.")]
        public int? TahminiTeslimGun { get; set; }

        // Sipariş detayları
        [Required(ErrorMessage = "Ürün adı girilmelidir.")]
        [Display(Name = "Ürün Adı")]
        public string UrunAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adet girilmelidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmalıdır.")]
        public int Adet { get; set; }

        [Required(ErrorMessage = "Birim fiyat girilmelidir.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Birim fiyat 0'dan büyük olmalıdır.")]
        [Display(Name = "Birim Fiyat (₺)")]
        public decimal BirimFiyat { get; set; }
    }
}
