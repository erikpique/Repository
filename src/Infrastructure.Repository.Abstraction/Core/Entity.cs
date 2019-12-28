using System;

namespace Infrastructure.Repository.Abstraction.Core
{
    public abstract class Entity<TKey> : AuditableEntity, IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right) => left is null || right is null ? false : left.Equals(right);

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => left is null || right is null ? false : !left.Equals(right);

        public bool Equals(TKey other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (Id is null && obj is null)
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            var other = obj as Entity<TKey>;

            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 13;
                hash = (hash * 7) ^ Id.GetHashCode();

                return hash;
            }
        }
    }
}
