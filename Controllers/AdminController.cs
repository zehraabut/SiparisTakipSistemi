using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SiparisTakipSistemi.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SiparisTakipSistemi.Controllers
{
    public class AdminController : Controller
    {
        private readonly SiparisTakipDbContext _context;

        public AdminController(SiparisTakipDbContext context)
        {
            _context = context;
        }

        // Excel Raporu İndir
        public IActionResult SiparisleriExcelIndir()
        {
            var siparisler = _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.Kargo)
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Siparişler");

            worksheet.Cell(1, 1).Value = "Sipariş ID";
            worksheet.Cell(1, 2).Value = "Kullanıcı Adı";
            worksheet.Cell(1, 3).Value = "Kargo Takip No";
            worksheet.Cell(1, 4).Value = "Sipariş Tarihi";
            worksheet.Cell(1, 5).Value = "Toplam Tutar";

            int row = 2;
            foreach (var s in siparisler)
            {
                worksheet.Cell(row, 1).Value = s.SiparisID;
                worksheet.Cell(row, 2).Value = s.Kullanici.KullaniciAdi;
                worksheet.Cell(row, 3).Value = s.Kargo.TakipNumarasi;
                worksheet.Cell(row, 4).Value = s.SiparisTarihi.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 5).Value = s.ToplamTutar;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Siparisler.xlsx");
        }

        // PDF Raporu İndir
        public IActionResult SiparisleriPdfIndir()
        {
            var siparisler = _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.Kargo)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Sipariş Listesi")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Kullanıcı");
                                header.Cell().Element(CellStyle).Text("Kargo Takip No");
                                header.Cell().Element(CellStyle).Text("Sipariş Tarihi");
                                header.Cell().Element(CellStyle).Text("Toplam Tutar");
                            });

                            foreach (var s in siparisler)
                            {
                                table.Cell().Element(CellStyle).Text(s.SiparisID.ToString());
                                table.Cell().Element(CellStyle).Text(s.Kullanici.KullaniciAdi);
                                table.Cell().Element(CellStyle).Text(s.Kargo.TakipNumarasi);
                                table.Cell().Element(CellStyle).Text(s.SiparisTarihi.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text($"{s.ToplamTutar:C}");
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", "Siparisler.pdf");
        }

        public IActionResult Kullanicilar()
        {
            // Veritabanından kullanıcıları çekiyoruz
            var kullanicilarListesi = _context.Kullanicilar.ToList();

            // View'a model olarak gönderiyoruz
            return View(kullanicilarListesi);
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            var popularKargo = _context.Kargolar
    .Include(k => k.KargoFirmasi)
    .GroupBy(k => k.KargoFirmasi != null ? k.KargoFirmasi.FirmaAdi : "Bilinmiyor")
    .Select(g => new
    {
        Firma = g.Key,
        Adet = g.Count()
    })
    .OrderByDescending(x => x.Adet)
    .ToList();




            var aylikSiparisler = _context.Siparisler
                .AsEnumerable()
                .GroupBy(s => new { s.SiparisTarihi.Year, s.SiparisTarihi.Month })
                .Select(g => new
                {
                    Ay = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Adet = g.Count()
                })
                .OrderBy(x => x.Ay)
                .ToList();

            ViewBag.PopularKargo = popularKargo;
            ViewBag.AylikSiparisler = aylikSiparisler;

            return View();
        }

        

        public IActionResult KullaniciyiSil(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            var kullanici = _context.Kullanicilar.Find(id);
            if (kullanici == null)
                return NotFound();

            // Kullanıcının siparişlerini al
            var siparisler = _context.Siparisler
                .Where(s => s.KullaniciID == id)
                .ToList();

            foreach (var siparis in siparisler)
            {
                // 1. Siparişe ait detayları sil
                var detaylar = _context.SiparisDetaylari
                    .Where(d => d.SiparisID == siparis.SiparisID)
                    .ToList();
                _context.SiparisDetaylari.RemoveRange(detaylar);

                // 2. Siparişi sil
                _context.Siparisler.Remove(siparis);

                // 3. Kargoyu sonra sil (önce sipariş silinmiş olmalı)
                var kargo = _context.Kargolar.FirstOrDefault(k => k.KargoID == siparis.KargoID);
                if (kargo != null)
                {
                    _context.Kargolar.Remove(kargo);
                }
            }

            // 4. Kullanıcıyı sil
            _context.Kullanicilar.Remove(kullanici);

            _context.SaveChanges();
            return RedirectToAction("Kullanicilar");
        }



        public IActionResult AdminiSil(int id)
        {
            var admin = _context.Kullanicilar.Find(id);
            if (admin != null && admin.Rol == "Admin")
            {
                _context.Kullanicilar.Remove(admin);
                _context.SaveChanges();
            }
            return RedirectToAction("Adminler");
        }

        public IActionResult Siparisler(int? kullaniciId)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            ViewBag.Kullanicilar = _context.Kullanicilar.ToList();

            var siparisler = _context.Siparisler
                .Include(s => s.Kargo)
                    .ThenInclude(k => k.KargoFirmasi)
                .Include(s => s.Kullanici)
                .AsQueryable();

            if (kullaniciId.HasValue)
            {
                siparisler = siparisler.Where(s => s.KullaniciID == kullaniciId.Value);
            }

            ViewBag.SeciliKullaniciId = kullaniciId;

            return View(siparisler.ToList());
        }


        public IActionResult SiparisDetay(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            var detaylar = _context.SiparisDetaylari
                .Where(d => d.SiparisID == id)
                .ToList();

            ViewBag.SiparisID = id;
            return View(detaylar);
        }

    



        public IActionResult SiparisiSil(int id)
        {
            var siparis = _context.Siparisler
                .Include(s => s.SiparisDetaylaris)
                .FirstOrDefault(s => s.SiparisID == id);

            if (siparis != null)
            {
                _context.SiparisDetaylari.RemoveRange(siparis.SiparisDetaylaris);
                _context.Siparisler.Remove(siparis);
                _context.SaveChanges();
            }

            return RedirectToAction("Siparisler");
        }

        [HttpGet]
        public IActionResult KullaniciEkle()
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            return View();
        }

        [HttpPost]
        public IActionResult KullaniciEkle(Kullanicilar model)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            if (ModelState.IsValid)
            {
                var varMi = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == model.KullaniciAdi);
                if (varMi != null)
                {
                    ViewBag.Hata = "Bu kullanıcı adı zaten mevcut.";
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.Rol))
                    model.Rol = "Kullanıcı";

                _context.Kullanicilar.Add(model);
                _context.SaveChanges();

                TempData["Basarili"] = "Yeni kullanıcı başarıyla eklendi.";
                return RedirectToAction("Kullanicilar");
            }

            ViewBag.Hata = "Lütfen tüm alanları doğru doldurun.";
            return View(model);
        }

        // Kargo Listeleme (Sipariş, Kargo ve Kullanıcı bilgileriyle)
        public IActionResult Kargolar(int? kullaniciId)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            var siparisler = _context.Siparisler
                .Include(s => s.Kargo)
                    .ThenInclude(k => k.KargoFirmasi)
                .Include(s => s.Kullanici)
                .ToList();

            if (kullaniciId.HasValue)
                siparisler = siparisler.Where(s => s.KullaniciID == kullaniciId.Value).ToList();

            ViewBag.Kullanicilar = _context.Kullanicilar.ToList();
            ViewBag.SeciliKullaniciId = kullaniciId;

            return View(siparisler);
        }



        // Kargo Silme
        public IActionResult KargoSil(int id)
        {
            var kargo = _context.Kargolar.Find(id);
            if (kargo != null)
            {
                _context.Kargolar.Remove(kargo);
                _context.SaveChanges();
            }
            return RedirectToAction("Kargolar");
        }

        [HttpGet]
        public IActionResult KargoEkle()
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            ViewBag.Kullanicilar = _context.Kullanicilar.ToList();
            ViewBag.KargoFirmalari = _context.KargoFirmalari.ToList();

            return View(new KargoEkleViewModel());
        }

        [HttpPost]
        public IActionResult KargoEkle(KargoEkleViewModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            if (ModelState.IsValid)
            {
                // 1. Kargo oluştur
                var kargo = new Kargolar
                {
                    KullaniciID = model.KullaniciID,
                    KargoFirmaID = model.KargoFirmaID,
                    TakipNumarasi = model.TakipNumarasi,
                    Durum = model.Durum,
                    Konum = model.Konum,
                    TahminiTeslimGun = model.TahminiTeslimGun
                };
                _context.Kargolar.Add(kargo);
                _context.SaveChanges();

                // 2. Sipariş oluştur
                var siparis = new Siparisler
                {
                    KullaniciID = model.KullaniciID,
                    KargoID = kargo.KargoID,
                    SiparisTarihi = DateTime.Now,
                    ToplamTutar = model.Adet * model.BirimFiyat
                };
                _context.Siparisler.Add(siparis);
                _context.SaveChanges();

                // 3. Sipariş detayı oluştur
                var detay = new SiparisDetaylari
                {
                    SiparisID = siparis.SiparisID,
                    UrunAdi = model.UrunAdi,
                    Adet = model.Adet,
                    BirimFiyat = model.BirimFiyat
                };
                _context.SiparisDetaylari.Add(detay);
                _context.SaveChanges();

                return RedirectToAction("Kargolar");
            }

            // ModelState geçersizse ViewBag'leri yeniden yükle
            ViewBag.Kullanicilar = _context.Kullanicilar.ToList();
            ViewBag.KargoFirmalari = _context.KargoFirmalari.ToList();
            return View(model);
        }






        // Kargo Düzenleme Formu (GET)
        [HttpGet]
        public IActionResult KargoDuzenle(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            var siparis = _context.Siparisler
                .Include(s => s.Kargo)
                .Include(s => s.Kullanici)
                .FirstOrDefault(s => s.SiparisID == id);

            if (siparis == null)
                return NotFound();

            ViewBag.Kullanicilar = _context.Kullanicilar.ToList();
            ViewBag.KargoFirmalari = _context.KargoFirmalari.ToList();

            return View(siparis);
        }



        // Kargo Düzenleme İşlemi (POST)
        [HttpPost]
        public IActionResult KargoDuzenle(int SiparisID, int KullaniciID, int KargoFirmaID, Kargolar kargoModel)
        {
            if (!IsAdmin())
                return RedirectToAction("Giris", "Login");

            var siparis = _context.Siparisler
                .Include(s => s.Kargo)
                .FirstOrDefault(s => s.SiparisID == SiparisID);

            if (siparis == null)
                return NotFound();

            // Kargo bilgilerini güncelle
            var kargo = siparis.Kargo;
            kargo.KargoFirmaID = KargoFirmaID; 
            kargo.TakipNumarasi = kargoModel.TakipNumarasi;
            kargo.Durum = kargoModel.Durum;
            kargo.Konum = kargoModel.Konum;
            kargo.TahminiTeslimGun = kargoModel.TahminiTeslimGun;

            siparis.KullaniciID = KullaniciID;

            _context.SaveChanges();

            return RedirectToAction("Kargolar");
        }



        private bool IsAdmin()
        {
            var rol = HttpContext.Session.GetString("Rol");
            return rol != null && rol.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
