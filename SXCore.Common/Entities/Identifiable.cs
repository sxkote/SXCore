using SXCore.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Entities
{
    public abstract class IdentifiableEntity<T> : IIdentifiable<T>
    {
        public virtual T ID { get; protected set; }

        #region Equality
        /// <summary>
        /// Determines whether the specified entity is equal to the current instance.
        /// </summary>
        /// <param name="entity">An <see cref="System.Object"/> that 
        /// will be compared to the current instance.</param>
        /// <returns>True if the passed in entity is equal to the 
        /// current instance.</returns>
        public override bool Equals(object entity)
        {
            return entity != null && entity is IdentifiableEntity<T> && this == (IdentifiableEntity<T>)entity;
        }

        /// <summary>
        /// Operator overload for determining equality.
        /// </summary>
        /// <param name="base1">The first instance of an 
        /// <see cref="BaseEntity"/>.</param>
        /// <param name="base2">The second instance of an 
        /// <see cref="BaseEntity"/>.</param>
        /// <returns>True if equal.</returns>
        public static bool operator ==(IdentifiableEntity<T> base1, IdentifiableEntity<T> base2)
        {
            // check for both null (cast to object or recursive loop)
            if ((object)base1 == null && (object)base2 == null)
            {
                return true;
            }

            // check for either of them == to null
            if ((object)base1 == null || (object)base2 == null)
            {
                return false;
            }

            if (!base1.ID.Equals(base2.ID))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Operator overload for determining inequality.
        /// </summary>
        /// <param name="base1">The first instance of an 
        /// <see cref="BaseEntity"/>.</param>
        /// <param name="base2">The second instance of an 
        /// <see cref="BaseEntity"/>.</param>
        /// <returns>True if not equal.</returns>
        public static bool operator !=(IdentifiableEntity<T> base1, IdentifiableEntity<T> base2)
        {
            return !(base1 == base2);
        }

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        /// <returns>A hash code for the current Key 
        /// property.</returns>
        public override int GetHashCode()
        {
            if (this.ID != null)
            {
                return this.ID.GetHashCode();
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
