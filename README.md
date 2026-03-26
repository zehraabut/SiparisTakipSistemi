# Sipariş Takip Sistemi

Bu proje ASP.NET Core MVC kullanılarak geliştirilmiş bir sipariş takip sistemidir.  
Kullanıcılar sisteme kayıt olup giriş yapabilir ve kendi siparişlerini takip edebilir.  
Admin paneli üzerinden kullanıcı ve sipariş yönetimi yapılabilir.

## Kullanılan Teknolojiler

- ASP.NET Core MVC
- C#
- Entity Framework Core
- Microsoft SQL Server
- Bootstrap
- jQuery
- QuestPDF (PDF raporlama)
- ClosedXML (Excel raporlama)

## Özellikler

- Kullanıcı kayıt ve giriş sistemi
- Admin paneli
- Sipariş yönetimi
- Kargo firması yönetimi
- Sipariş detay görüntüleme
- PDF rapor oluşturma
- Excel rapor oluşturma
- Kullanıcı bazlı sipariş filtreleme

## Proje Yapısı
Controllers → Uygulama kontrol mantığı
Models → Veritabanı modelleri
Views → Razor sayfaları
wwwroot → CSS, JS ve statik dosyalar


## Kurulum

1. Projeyi klonlayın
git clone https://github.com/zehraabut/SiparisTakipSistemi.git

2. SQL Server üzerinde veritabanını oluşturun

3. appsettings.json dosyasındaki connection string'i düzenleyin

4. Migrationları çalıştırın
Update-Database

5. Uygulamayı çalıştırın
dotnet run
