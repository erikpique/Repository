namespace Infrastructure.Repository.Abstraction.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ValueObject<T>
        where T : ValueObject<T>
    {
        public static bool operator ==(ValueObject<T> left, ValueObject<T> right) => Equals(left, right);

        public static bool operator !=(ValueObject<T> left, ValueObject<T> right) => !(left == right);

        public override bool Equals(object other)
        {
            return Equals(other as T);
        }

        public virtual bool Equals(T other)
        {
            if (other == null)
            {
                return false;
            }

            return GetAttributesToIncludeInEqualityCheck().SequenceEqual(other.GetAttributesToIncludeInEqualityCheck());
        }

        public override int GetHashCode()
        {
            var hash = 17;

            foreach (var obj in GetAttributesToIncludeInEqualityCheck())
            {
                hash = (hash * 31) + (obj == null ? 0 : obj.GetHashCode());
            }

            return hash;
        }

        protected abstract IEnumerable<object> GetAttributesToIncludeInEqualityCheck();
    }
}
