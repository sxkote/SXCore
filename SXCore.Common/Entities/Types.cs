using SXCore.Common.Interfaces;

namespace SXCore.Common.Entities
{
    public abstract class Types : Entity, IType
    {
        public string Name { get; protected set; }
        public string Title { get; protected set; }

        public Types()
        {
        }

        public override string ToString()
        {
            return this.Title ?? this.Name ?? "";
        }
    }
}
