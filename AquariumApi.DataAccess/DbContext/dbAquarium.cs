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
        public virtual DbSet<AquariumPhoto> TblAquariumPhoto { get; set; }
        public virtual DbSet<FishPhoto> TblFishPhoto { get; set; }
        public virtual DbSet<CameraConfiguration> TblCameraConfiguration { get; set; }
        public virtual DbSet<Feeding> TblFeeding { get; set; }
        public virtual DbSet<Fish> TblFish { get; set; }
        public virtual DbSet<Species> TblSpecies { get; set; }
        public virtual DbSet<AquariumDevice> TblDevice { get; set; }
        public virtual DbSet<AquariumUser> TblAccounts { get; set; }
        public virtual DbSet<BugReport> TblBugReports { get; set; }
        public virtual DbSet<AquariumProfile> TblAquariumProfiles { get; set; }
        public virtual DbSet<Activity> TblAccountActivity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AquariumUser>(entity =>
            {
                entity.ToTable("tblAccount");
                entity.HasMany(e => e.Aquariums);
            });
            modelBuilder.Entity<Aquarium>(entity =>
            {

                entity.ToTable("tblAquarium");
                entity.HasKey(e => new { e.Id });
                entity.HasMany(e => e.Fish);
                entity.HasMany(e => e.Feedings);
            });
            modelBuilder.Entity<AquariumSnapshot>(entity =>
            {
                entity.ToTable("tblSnapshot");
                entity.HasKey(e => new { e.Id });
                entity.HasOne(e => e.Photo).WithOne(e => e.Snapshot);
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
            modelBuilder.Entity<FishPhoto>(entity =>
            {
                entity.ToTable("tblFishPhoto");
                entity.HasOne(e => e.Fish).WithMany(e => e.Photos);
            });
            modelBuilder.Entity<FishSnapshot>(entity =>
            {
                entity.ToTable("tblFishSnapshot");
                entity.HasOne(e => e.Fish).WithMany(e => e.Snapshots);
                entity.HasOne(e => e.AquariumSnapshot);
                entity.HasOne(e => e.FishPhoto);
            });

            modelBuilder.Entity<AquariumDevice>(entity =>
            {
                entity.ToTable("tblDevice");
                entity.HasOne(e => e.Aquarium).WithOne(e => e.Device);
                entity.HasOne(e => e.CameraConfiguration);

            });
            modelBuilder.Entity<CameraConfiguration>(entity =>
            {
                entity.ToTable("tblCameraConfiguration");
                //entity.HasOne(e => e.Device).WithOne(e => e.CameraConfiguration);
            });

            modelBuilder.Entity<AquariumPhoto>(entity =>
            {
                entity.ToTable("tblAquariumPhoto");
                entity.HasOne(e => e.Aquarium);
            });
            modelBuilder.Entity<FishPhoto>(entity =>
            {
                entity.ToTable("tblFishPhoto");
                entity.HasOne(e => e.Aquarium);
                entity.HasOne(e => e.Fish);
            });
            modelBuilder.Entity<BugReport>(entity =>
            {
                entity.ToTable("tblBugReports");
                entity.HasOne(e => e.ImpactedUser);
            });
            modelBuilder.Entity<AquariumProfile>(e =>
            {
                e.HasOne(ef => ef.Account).WithOne(ef => ef.Profile);
            });
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("tblAccountActivity");

            });
        }
    }
}