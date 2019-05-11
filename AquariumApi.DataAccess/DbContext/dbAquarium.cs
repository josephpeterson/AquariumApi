using Microsoft.EntityFrameworkCore;

namespace AquariumApi.DataAccess
{
    public partial class DbAquariumContext : DbContext
    {
        
        public DbAquariumContext(DbContextOptions<DbAquariumContext> options) : base(options)
        {
            //if (Database.IsSqlServer()) Database.SetCommandTimeout(280);
        }

        public virtual DbSet<tblTank> TblTank { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblTank>(entity =>
            {
                entity.HasKey(e => new { e.Id })
                    .HasName("PK_tblTank");

                entity.ToTable("tblTank");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("int");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
                entity.Property(e => e.Gallons)
                    .IsRequired()
                    .HasColumnType("int");
                entity.Property(e => e.StartDate)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(e => e.Type)
                    .HasColumnType("varchar(50)");
            });

        }
    }
}