using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace EcommerceChitarre.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext1")
        {
        }

        public virtual DbSet<Articoli> Articoli { get; set; }
        public virtual DbSet<Ordini> Ordini { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<OrdArt> OrdArt { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articoli>()
                .Property(e => e.Prezzo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Articoli>()
                .Property(e => e.Dettagli)
                .IsUnicode(false);

            modelBuilder.Entity<Articoli>()
                .HasMany(e => e.OrdArt)
                .WithRequired(e => e.Articoli)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ordini>()
                .Property(e => e.CostoCons)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ordini>()
                .HasMany(e => e.OrdArt)
                .WithRequired(e => e.Ordini)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Ordini)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);
        }
    }
}
