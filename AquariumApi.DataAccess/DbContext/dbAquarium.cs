using AquariumApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AquariumApi.DataAccess
{
    public partial class DbAquariumContext : DbContext
    {
        
        public DbAquariumContext(DbContextOptions<DbAquariumContext> options) : base(options)
        {
            //if (Database.IsSqlServer()) Database.SetCommandTimeout(280);
        }

        public virtual DbSet<Aquarium> TblAquarium { get; set; }
        public virtual DbSet<AquariumSnapshot> TblSnapshot { get; set; }
        public virtual DbSet<tblWaterParameters> TblWaterParameters { get; set; }
        public virtual DbSet<CameraConfiguration> TblCameraConfiguration { get; set; }
        public virtual DbSet<Feeding> TblFeeding { get; set; }
        public virtual DbSet<Fish> TblFish { get; set; }
        public virtual DbSet<Species> TblSpecies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aquarium>(entity =>
            {

                entity.ToTable("tblAquarium");
                entity.HasKey(e => new { e.Id });
                entity.HasOne(e => e.CameraConfiguration);
                entity.HasMany(e => e.Fish);
                entity.HasMany(e => e.Feedings);
            });
            modelBuilder.Entity<AquariumSnapshot>(entity =>
            {
                entity.ToTable("tblSnapshot");
                entity.HasKey(e => new { e.Id });
            });
            modelBuilder.Entity<CameraConfiguration>(entity =>
            {
                entity.ToTable("tblCameraConfiguration");
            });
            modelBuilder.Entity<Feeding>(entity =>
            {
                entity.ToTable("tblFeeding");
                entity.HasOne(e => e.Aquarium);
                entity.HasOne(e => e.Fish).WithMany(f => f.Feedings);
            });
            modelBuilder.Entity<Fish>(entity =>
            {
                entity.ToTable("tblFish");
                entity.HasOne(e => e.Aquarium);
                entity.HasOne(e => e.Species);
            });
            modelBuilder.Entity<Species>(entity =>
            {
                entity.ToTable("tblSpecies");
            });
        }
    }
}