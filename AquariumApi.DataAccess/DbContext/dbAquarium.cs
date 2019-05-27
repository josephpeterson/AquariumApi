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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aquarium>(entity =>
            {

                entity.ToTable("tblAquarium");
                entity.HasKey(e => new { e.Id });
                entity.HasOne(e => e.CameraConfiguration);
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
        }
    }
}