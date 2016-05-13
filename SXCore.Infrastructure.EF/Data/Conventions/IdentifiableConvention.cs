using SXCore.Common.Interfaces;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace SXCore.Infrastructure.EF.Data.Conventions
{
    public class IdentifiableConvention : Convention
    {
        public IdentifiableConvention()
        {
            var interfaceType = typeof(IIdentifiable<>);
            this.Types()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType))
                .Configure(t => t.HasKey("ID"));
        }
    }
}
