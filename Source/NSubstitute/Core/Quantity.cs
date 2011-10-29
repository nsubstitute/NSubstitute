using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public abstract class Quantity
    {
        public static Quantity Exactly(int number) { return new ExactQuantity(number); }
        public static Quantity AtLeastOne() { return new AtLeastQuantity(1); }
        public static Quantity AtLeast(int number) { return new AtLeastQuantity(number); }
        public static Quantity None() { return new NoneQuantity(); }

        public abstract bool Matches<T>(IEnumerable<T> items);

        private class ExactQuantity : Quantity
        {
            private readonly int _number;
            public ExactQuantity(int number) { _number = number; }
            public override bool Matches<T>(IEnumerable<T> items)
            {
                return _number == items.Count();
            }

            public bool Equals(ExactQuantity other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other._number == _number;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (ExactQuantity)) return false;
                return Equals((ExactQuantity) obj);
            }

            public override int GetHashCode() { return _number; }
        }

        private class AtLeastQuantity : Quantity
        {
            private readonly int _number;
            public AtLeastQuantity(int number) { _number = number; }
            public override bool Matches<T>(IEnumerable<T> items)
            {
                return _number == items.Take(_number).Count();
            }

            public bool Equals(AtLeastQuantity other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other._number == _number;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (AtLeastQuantity)) return false;
                return Equals((AtLeastQuantity) obj);
            }

            public override int GetHashCode() { return _number; }
        }

        private class NoneQuantity : Quantity
        {
            public override bool Matches<T>(IEnumerable<T> items) { return !items.Any(); }

            public bool Equals(NoneQuantity other)
            {
                return !ReferenceEquals(null, other);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (NoneQuantity)) return false;
                return Equals((NoneQuantity) obj);
            }

            public override int GetHashCode() { return 0; }
        }
    }
}