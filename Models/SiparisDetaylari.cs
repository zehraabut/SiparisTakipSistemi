using System;
using System.Collections.Generic;

namespace SiparisTakipSistemi.Models;

public partial class SiparisDetaylari
{
    public int DetayID { get; set; }

    public int SiparisID { get; set; }

    public string UrunAdi { get; set; } = null!;

    public int Adet { get; set; }

    public decimal BirimFiyat { get; set; }


    public virtual Siparisler Siparis { get; set; } = null!;
}
