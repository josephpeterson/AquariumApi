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
        public virtual DbSet<Equipment> TblAquariumEquipment { get; set; }
        public virtual DbSet<Substrate> TblAquariumSubstrate { get; set; }
        public virtual DbSet<AquariumPlan> TblAquariumPlan { get; set; }
        public virtual DbSet<AquariumSnapshot> TblSnapshot { get; set; }
        public virtual DbSet<AquariumPhoto> TblAquariumPhoto { get; set; }
        public virtual DbSet<FishPhoto> TblFishPhoto { get; set; }
        public virtual DbSet<CameraConfiguration> TblCameraConfiguration { get; set; }
        public virtual DbSet<Feeding> TblFeeding { get; set; }
        public virtual DbSet<Fish> TblFish { get; set; }
        public virtual DbSet<Species> TblSpecies { get; set; }
        public virtual DbSet<AquariumDevice> TblDevice { get; set; }
        public virtual DbSet<SignupRequest> TblSignupRequests { get; set; }
        public virtual DbSet<AquariumUser> TblAccounts { get; set; }
        public virtual DbSet<BugReport> TblBugReports { get; set; }
        public virtual DbSet<AquariumProfile> TblAquariumProfiles { get; set; }
        public virtual DbSet<Activity> TblAccountActivity { get; set; }
        public virtual DbSet<AccountRelationship> TblAccountRelationship { get; set; }

        public virtual DbSet<PostCategory> TblPostCategories { get; set; }

        public virtual DbSet<PostBoard> TblPostBoards { get; set; }

        public virtual DbSet<PostThread> TblPostThreads { get; set; }
        public virtual DbSet<Post> TblPosts { get; set; }
        public virtual DbSet<PostReaction> TblPostReactions { get; set; }
        public virtual DbQuery<PostBoardView> vwPostBoards { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AquariumUser>(entity =>
            {
                entity.ToTable("tblAccount");
                entity.HasMany(e => e.Aquariums);
                
            });
            modelBuilder.Entity<SignupRequest>(entity =>
            {
                entity.ToTable("tblAccount");
                entity.HasOne(o => o.Account).WithOne()
                    .HasForeignKey<AquariumUser>(o => o.Id);
            });
            modelBuilder.Entity<SignupRequest>(entity =>
            {
                entity.ToTable("tblAccount");
                entity.HasOne(o => o.Account).WithOne()
                    .HasForeignKey<AquariumUser>(o => o.Id);
            });

            modelBuilder.Entity<Aquarium>(entity =>
            {

                entity.ToTable("tblAquarium");
                entity.HasKey(e => new { e.Id });
                entity.HasMany(e => e.Fish);
                entity.HasMany(e => e.Feedings);

                entity.HasOne(o => o.Substrate).WithOne(s => s.Aquarium)
                    .HasForeignKey<Substrate>(o => o.Id);
                entity.HasMany(e => e.Equipment).WithOne(e => e.Aquarium);
                entity.HasOne(o => o.Plan).WithOne(s => s.Aquarium)
                    .HasForeignKey<AquariumPlan>(o => o.Id);

            });
            modelBuilder.Entity<Substrate>(entity =>
            {
            });
            modelBuilder.Entity<Equipment>(entity =>
            {
            });
            modelBuilder.Entity<AquariumPlan>(entity =>
            {
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
            modelBuilder.Entity<AccountRelationship>(entity =>
            {
                entity.ToTable("tblAccountRelationship");

            });

            /* Posts */
            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.ToTable("tblPostCategory");
            });
            modelBuilder.Entity<PostBoard>(entity =>
            {
                entity.ToTable("tblPostBoard");

            });
            modelBuilder.Entity<PostThread>(entity =>
            {
                entity.ToTable("tblPostThread");

            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("tblPost");

            });
            modelBuilder.Entity<PostReaction>(entity =>
            {
                entity.ToTable("tblPostReaction");

            });

            modelBuilder.Query<PostBoardView>().ToView("vw_PostBoards");
        }
    }
}