using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Taxes.Models.DbContexts {
    public class TaxesDbContext : DbContext {
        public TaxesDbContext() : base("Connection") {
            //Database.SetInitializer<TaxesDbContext>(null);
            Configure(this);
        }

        public TaxesDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection) {
            Database.SetInitializer<TaxesDbContext>(null);
            Configure(this);
        }

        public DbSet<Municipality> Municipalities { get; set; }

        public DbSet<Tax> Taxes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            SetConventions(modelBuilder);

            modelBuilder.Entity<Municipality>()
                .HasMany(m => m.Taxes)
                .WithRequired()
                .HasForeignKey(m => m.Municipality)
                .WillCascadeOnDelete(true);

            var taxCfg = modelBuilder.Entity<Tax>();
            taxCfg.Property(t => t.DateFrom).HasColumnType("date");
            taxCfg.Property(t => t.DateTo).HasColumnType("date");

            modelBuilder.Properties<DateTime>()
                        .Configure(p => p.HasColumnType("datetime2"));
        }

        public static void Configure(DbContext context) {
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
        }

        public static DbModelBuilder SetConventions(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();

            return modelBuilder;
        }
    }
}