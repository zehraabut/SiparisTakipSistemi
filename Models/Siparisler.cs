using System;
using System.Collections.Generic;

namespace SiparisTakipSistemi.Models;

public partial class Siparisler
{
    public int SiparisID { get; set; }

    public int KullaniciID { get; set; }

    public DateTime SiparisTarihi { get; set; }

    public int KargoID { get; set; }

    public decimal ToplamTutar { get; set; }

    public virtual Kargolar Kargo { get; set; } = null!;

    public virtual Kullanicilar Kullanici { get; set; } = null!;

    public virtual ICollection<SiparisDetaylari> SiparisDetaylaris { get; } = new List<SiparisDetaylari>();
}
