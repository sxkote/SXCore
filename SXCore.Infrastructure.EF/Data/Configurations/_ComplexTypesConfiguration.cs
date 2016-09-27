using SXCore.Common.Values;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace SXCore.Infrastructure.EF.Data.Configurations
{
    public class FileNameComplexTypeConfiguration : ComplexTypeConfiguration<FileName>
    {
        public FileNameComplexTypeConfiguration()
        {
            this.Property(p => p.Name).HasColumnName("FileName");
            this.Ignore(p => p.Type);
            this.Ignore(p => p.MimeType);
            this.Ignore(p => p.Extension);
        }
    }

    public class PersonNameComplexTypeConfiguration : ComplexTypeConfiguration<PersonName>
    {
        public PersonNameComplexTypeConfiguration()
        {
            this.Property(p => p.First).HasColumnName("NameFirst");
            this.Property(p => p.Last).HasColumnName("NameLast");
        }
    }

    public class PersonFullNameComplexTypeConfiguration : ComplexTypeConfiguration<PersonFullName>
    {
        public PersonFullNameComplexTypeConfiguration()
        {
            this.Property(p => p.First).HasColumnName("NameFirst");
            this.Property(p => p.Last).HasColumnName("NameLast");
            this.Property(p => p.Second).HasColumnName("NameSecond");
        }
    }

    public class PersonTotalNameComplexTypeConfiguration : ComplexTypeConfiguration<PersonTotalName>
    {
        public PersonTotalNameComplexTypeConfiguration()
        {
            this.Property(p => p.First).HasColumnName("NameFirst");
            this.Property(p => p.Last).HasColumnName("NameLast");
            this.Property(p => p.Second).HasColumnName("NameSecond");
            this.Property(p => p.Maiden).HasColumnName("NameMaiden");
        }
    }

    //public class PeriodComplexTypeConfiguration : ComplexTypeConfiguration<Period>
    //{
    //    public PeriodComplexTypeConfiguration()
    //    {
    //        this.Property(p => p.Begin).HasColumnName("PeriodBegin");
    //        this.Property(p => p.End).HasColumnName("PeriodEnd");
    //    }
    //}
}
