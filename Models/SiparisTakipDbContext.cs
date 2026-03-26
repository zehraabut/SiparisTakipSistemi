using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SiparisTakipSistemi.Models
{
    public partial class SiparisTakipDbContext : DbContext
    {
        public SiparisTakipDbContext()
        {
        }

        public SiparisTakipDbContext(DbContextOptions<SiparisTakipDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Kargolar> Kargolar { get; set; }

        public virtual DbSet<Kullanicilar> Kullanicilar { get; set; }

        public virtual DbSet<SiparisDetaylari> SiparisDetaylari { get; set; }

        public virtual DbSet<Siparisler> Siparisler { get; set; }

        public virtual DbSet<KargoFirmalari> KargoFirmalari { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kargolar>(entity =>
            {
                entity.HasKey(e => e.KargoID).HasName("PK__Kargolar__8E5A959B36615701");

                entity.ToTable("Kargolar");

                entity.Property(e => e.Durum).HasMaxLength(100);

                entity.Property(e => e.Konum).HasMaxLength(100);
                entity.Property(e => e.TakipNumarasi).HasMaxLength(50);

                entity.HasOne(e => e.KargoFirmasi) 
                    .WithMany(f => f.Kargolar)
                    .HasForeignKey(e => e.KargoFirmaID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Kargolar_KargoFirmalari");

                
            });

            modelBuilder.Entity<Kullanicilar>(entity =>
            {
                entity.HasKey(e => e.KullaniciID).HasName("PK__Kullanic__E011F09BF005D59A");

                entity.ToTable("Kullanicilar");

                entity.Property(e => e.Eposta).HasMaxLength(100);
                entity.Property(e => e.KullaniciAdi).HasMaxLength(100);
                entity.Property(e => e.Rol).HasMaxLength(20);
                entity.Property(e => e.Sifre).HasMaxLength(255);
            });

            modelBuilder.Entity<SiparisDetaylari>(entity =>
            {
                entity.HasKey(e => e.DetayID).HasName("PK__SiparisD__8E8164A5A741476F");

                entity.ToTable("SiparisDetaylari");

                entity.Property(e => e.BirimFiyat).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.UrunAdi).HasMaxLength(150);

                entity.HasOne(d => d.Siparis).WithMany(p => p.SiparisDetaylaris)
                    .HasForeignKey(d => d.SiparisID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SiparisDe__Sipar__412EB0B6");
            });

            modelBuilder.Entity<Siparisler>(entity =>
            {
                entity.HasKey(e => e.SiparisID).HasName("PK__Siparisl__C3F03BDD9675DC02");

                entity.ToTable("Siparisler");

                entity.Property(e => e.SiparisTarihi)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ToplamTutar).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Kargo).WithMany(p => p.Siparisler)
                    .HasForeignKey(d => d.KargoID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Siparisle__Kargo__3E52440B");

                entity.HasOne(d => d.Kullanici).WithMany(p => p.Siparislers)
                    .HasForeignKey(d => d.KullaniciID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Siparisle__Kulla__3D5E1FD2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
