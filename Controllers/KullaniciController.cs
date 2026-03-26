using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SiparisTakipSistemi.Models;
using System;
using System.Linq;

namespace SiparisTakipSistemi.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly SiparisTakipDbContext _context;

        public KullaniciController(SiparisTakipDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string kargoFirmasi = "", bool son30Gun = false)
        {
            var kullaniciAdi = HttpContext.Session.GetString("KullaniciAdi");
            var rol = HttpContext.Session.GetString("Rol");

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(rol))
                return RedirectToAction("Giris", "Login");

            if (!string.Equals(rol, "Kullanıcı", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Giris", "Login");

            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);
            if (kullanici == null)
                return RedirectToAction("Giris", "Login");

            var siparisler = _context.Siparisler
                .Include(s => s.Kargo)
                    .ThenInclude(k => k.KargoFirmasi)
                .Where(s => s.KullaniciID == kullanici.KullaniciID)
                .AsQueryable();

            if (!string.IsNullOrEmpty(kargoFirmasi))
            {
                siparisler = siparisler
                    .Where(s => s.Kargo != null &&
                                s.Kargo.KargoFirmasi != null &&
                                s.Kargo.KargoFirmasi.FirmaAdi == kargoFirmasi);
            }

            if (son30Gun)
            {
                var son30GunTarihi = DateTime.Now.AddDays(-30);
                siparisler = siparisler.Where(s => s.SiparisTarihi >= son30GunTarihi);
            }

            var liste = siparisler.ToList();

            ViewBag.KullaniciAdi = kullaniciAdi;
            ViewBag.SiparisSayisi = liste.Count;
            ViewBag.ToplamTutar = liste.Sum(s => s.ToplamTutar);
            ViewBag.SonSiparisTarihi = liste.OrderByDescending(s => s.SiparisTarihi).FirstOrDefault()?.SiparisTarihi;

            return View(liste);
        }


        public IActionResult SiparisDetay(int id)
        {
            var detaylar = _context.SiparisDetaylari
                .Where(d => d.SiparisID == id)
                .ToList();

            ViewBag.SiparisID = id;
            return View(detaylar);
        }
    }
}
