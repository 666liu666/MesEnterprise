using Microsoft.EntityFrameworkCore;
using MesEnterprise.Models;

namespace MesEnterprise.Data
{
    public class MesDbContext : DbContext
    {
        public MesDbContext(DbContextOptions<MesDbContext> options) : base(options) { }

        public DbSet<SysFactory> Factories { get; set; }
        public DbSet<SysHtFactory> HtFactories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SAJET");

            modelBuilder.Entity<SysFactory>(entity =>
            {
                entity.ToTable("SYS_FACTORY");
                entity.HasKey(e => e.FactoryId);

                entity.Property(e => e.FactoryId).HasColumnName("FACTORY_ID");
                entity.Property(e => e.FactoryCode).HasColumnName("FACTORY_CODE");
                entity.Property(e => e.FactoryName).HasColumnName("FACTORY_NAME");
                entity.Property(e => e.FactoryDesc).HasColumnName("FACTORY_DESC");
                entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
                entity.Property(e => e.UpdateTime).HasColumnName("UPDATE_TIME");
                entity.Property(e => e.Enabled).HasColumnName("ENABLED");
            });

            modelBuilder.Entity<SysHtFactory>(entity =>
            {
                entity.ToTable("SYS_HT_FACTORY");
                entity.HasKey(e => e.FactoryId);

                entity.Property(e => e.FactoryId).HasColumnName("FACTORY_ID");
                entity.Property(e => e.FactoryCode).HasColumnName("FACTORY_CODE");
                entity.Property(e => e.FactoryName).HasColumnName("FACTORY_NAME");
                entity.Property(e => e.FactoryDesc).HasColumnName("FACTORY_DESC");
                entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
                entity.Property(e => e.UpdateTime).HasColumnName("UPDATE_TIME");
                entity.Property(e => e.Enabled).HasColumnName("ENABLED");
            });
        }
    }
}
