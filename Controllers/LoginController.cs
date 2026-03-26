using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SiparisTakipSistemi.Models;
using System.Linq;

public class LoginController : Controller
{
    private readonly SiparisTakipDbContext _context;

    public LoginController(SiparisTakipDbContext context)
    {
        _context = context;
    }

    public IActionResult Giris()
    {
        if (TempData["Basarili"] != null)
            ViewBag.Basarili = TempData["Basarili"]?.ToString();

        if (TempData["Hata"] != null)
            ViewBag.Hata = TempData["Hata"]?.ToString();

        return View();
    }

    [HttpPost]
    public IActionResult Giris(string kullaniciAdi, string sifre)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
        {
            ViewBag.Hata = "Kullanıcı adı ve şifre boş olamaz.";
            return View();
        }

        var kullanici = _context.Kullanicilar
            .FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi.Trim() && k.Sifre == sifre.Trim());

        if (kullanici != null)
        {
            HttpContext.Session.SetString("KullaniciAdi", kullanici.KullaniciAdi);
            HttpContext.Session.SetString("Rol", kullanici.Rol);

            if (kullanici.Rol.Equals("Admin", System.StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Admin");

            if (kullanici.Rol.Equals("Kullanıcı", System.StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Kullanici");

            TempData["Hata"] = "Rol tanımlanamıyor.";
            return RedirectToAction("Giris");
        }

        ViewBag.Hata = "Kullanıcı adı veya şifre yanlış.";
        return View();
    }

    public IActionResult Kayit()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Kayit(Kullanicilar model)
    {
        if (ModelState.IsValid)
        {
            bool kullaniciAdiVar = _context.Kullanicilar.Any(k => k.KullaniciAdi == model.KullaniciAdi);

            if (kullaniciAdiVar)
            {
                ViewBag.Hata = "Bu kullanıcı adı zaten mevcut. Lütfen farklı bir kullanıcı adı seçin.";
                return View(model);
            }

            model.Rol = "Kullanıcı";

            _context.Kullanicilar.Add(model);
            _context.SaveChanges();

            TempData["Basarili"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction("Giris");
        }

        ViewBag.Hata = "Kayıt işlemi başarısız.";
        return View(model);
    }

    public IActionResult Cikis()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Giris");
    }
}