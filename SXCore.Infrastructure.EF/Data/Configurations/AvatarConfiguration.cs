using SXCore.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SXCore.Infrastructure.EF.Data.Configurations
{
    public class AvatarConfiguration : EntityTypeConfiguration<Avatar>
    {
        public AvatarConfiguration()
        {
            ToTable("Avatar");

            Property(t => t.ID)
                .HasColumnName("AvatarID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
