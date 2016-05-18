using SXCore.Common.Enums;
using SXCore.Common.Interfaces;
using SXCore.Infrastructure.EF.Data.Configurations;
using SXCore.Infrastructure.EF.Data.Conventions;
using System.Data.Entity;
using System.Linq;

namespace SXCore.Infrastructure.EF.Data
{
    public partial class CoreDbContext : DbContext
    {
        public CoreDbContext()
            : base("name=DBConnection")
        { }

        public CoreDbContext(string connection)
            : base(connection)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;

#if DEBUG
            this.Database.Log = (s) => System.Diagnostics.Debug.WriteLine(s);
#endif

            // Conventions
            modelBuilder.Conventions.Add(new IdentifiableConvention());

            // ComplexTypes
            modelBuilder.Configurations.Add(new FileNameComplexTypeConfiguration());
            modelBuilder.Configurations.Add(new PersonNameComplexTypeConfiguration());
            modelBuilder.Configurations.Add(new PersonFullNameComplexTypeConfiguration());
            modelBuilder.Configurations.Add(new PersonTotalNameComplexTypeConfiguration());
            modelBuilder.Configurations.Add(new PeriodComplexTypeConfiguration());

            // Entities
            modelBuilder.Configurations.Add(new AvatarConfiguration());
            modelBuilder.Configurations.Add(new FileBlobConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var entry in this.ChangeTracker.Entries<IDbEntity>().Where(e => e.Entity.DbState != DbState.None))
                entry.State = ConvertEntityState(entry.Entity.DbState);

            return base.SaveChanges();
        }

        static protected EntityState ConvertEntityState(DbState state)
        {
            switch (state)
            {
                case DbState.Added:
                    return EntityState.Added;
                case DbState.Modified:
                    return EntityState.Modified;
                case DbState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }
    }
}
