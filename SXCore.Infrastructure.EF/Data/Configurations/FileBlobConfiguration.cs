using SXCore.Common.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SXCore.Infrastructure.EF.Data.Configurations
{
    public class FileBlobConfiguration : EntityTypeConfiguration<FileBlob>
    {
        public FileBlobConfiguration()
        {
            ToTable("FileBlob");

            Property(t => t.ID)
                .HasColumnName("FileBlobID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }

}
