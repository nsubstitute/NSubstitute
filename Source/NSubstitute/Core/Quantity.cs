using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public abstract class Quantity
    {
        public static Quantity Exactly(int number) { return number == 0 ? None() : new ExactQuantity(number); }
        public static Quantity AtLeastOne() { return new AnyNonZeroQuantity(); }
        public static Quantity None() { return new NoneQuantity(); }

        public abstract bool Matches<T>(IEnumerable<T> items);
        public abstract bool RequiresMoreThan<T>(IEnumerable<T> items);
        public abstract string Describe(string singularNoun, string pluralNoun);

        private class ExactQuantity : Quantity
        {
            private readonly int _number;
            public ExactQuantity(int number) { _number = number; }
            public override bool Matches<T>(IEnumerable<T> items) { return _number == items.Count(); }
            public override bool RequiresMoreThan<T>(IEnumerable<T> items) { return _number > items.Count(); }
            public override string Describe(string singularNoun, string pluralNoun)
            {
                return string.Format("exactly {0} {1}", _number, _number == 1 ? singularNoun : pluralNoun);
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

        private class AnyNonZeroQuantity : Quantity {
            public override bool Matches<T>(IEnumerable<T> items) { return items.Any(); }
            public override bool RequiresMoreThan<T>(IEnumerable<T> items) { return !items.Any(); }

            public override string Describe(string singularNoun, string pluralNoun)
            {
                return string.Format("a {0}", singularNoun);
            }

            public bool Equals(AnyNonZeroQuantity other)
            {
                return !ReferenceEquals(null, other);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (AnyNonZeroQuantity)) return false;
                return Equals((AnyNonZeroQuantity) obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        private class NoneQuantity : Quantity
        {
            public override bool Matches<T>(IEnumerable<T> items) { return !items.Any(); }
            public override bool RequiresMoreThan<T>(IEnumerable<T> items) { return false; }
            public override string Describe(string singularNoun, string pluralNoun)
            {
                return "no " + pluralNoun;
            }

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