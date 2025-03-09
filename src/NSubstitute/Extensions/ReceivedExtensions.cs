using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.ReceivedExtensions;

public static class ReceivedExtensions
{
    /// <summary>
    /// Checks this substitute has received the following call the required number of times.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="substitute"></param>
    /// <param name="requiredQuantity"></param>
    /// <returns></returns>
    public static T Received<T>(this T substitute, Quantity requiredQuantity)
    {
        if (substitute == null) throw new NullSubstituteReferenceException();

        var context = SubstitutionContext.Current;
        var callRouter = context.GetCallRouterFor(substitute);

        context.ThreadContext.SetNextRoute(callRouter, x => context.RouteFactory.CheckReceivedCalls(x, MatchArgs.AsSpecifiedInCall, requiredQuantity));
        return substitute;
    }

    /// <summary>
    /// Checks this substitute has received the following call with any arguments the required number of times.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="substitute"></param>
    /// <param name="requiredQuantity"></param>
    /// <returns></returns>
    public static T ReceivedWithAnyArgs<T>(this T substitute, Quantity requiredQuantity)
    {
        if (substitute == null) throw new NullSubstituteReferenceException();

        var context = SubstitutionContext.Current;
        var callRouter = context.GetCallRouterFor(substitute);

        context.ThreadContext.SetNextRoute(callRouter, x => context.RouteFactory.CheckReceivedCalls(x, MatchArgs.Any, requiredQuantity));
        return substitute;
    }
}

/// <summary>
/// Represents a quantity. Primarily used for specifying a required amount of calls to a member.
/// </summary>
public abstract record Quantity
{
    public static Quantity Exactly(int number) { return number == 0 ? None() : new ExactQuantity(number); }
    public static Quantity AtLeastOne() { return new AnyNonZeroQuantity(); }
    public static Quantity None() { return new NoneQuantity(); }
    /// <summary>
    /// A non-zero quantity between the given minimum and maximum numbers (inclusive).
    /// </summary>
    /// <param name="minInclusive">Minimum quantity (inclusive). Must be greater than or equal to 0.</param>
    /// <param name="maxInclusive">Maximum quantity (inclusive). Must be greater than minInclusive.</param>
    /// <returns></returns>
    public static Quantity Within(int minInclusive, int maxInclusive) { return new RangeQuantity(minInclusive, maxInclusive); }

    /// <summary>
    /// Returns whether the given collection contains the required quantity of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns>true if the collection has the required quantity; otherwise false.</returns>
    public abstract bool Matches<T>(IEnumerable<T> items);

    /// <summary>
    /// Returns whether the given collections needs more items to satisfy the required quantity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns>true if the collection needs more items to match this quantity; otherwise false.</returns>
    public abstract bool RequiresMoreThan<T>(IEnumerable<T> items);

    /// <summary>
    /// Describe this quantity using the given noun variants.
    /// For example, `Describe("item", "items")` could return the description:
    /// "more than 1 item, but less than 10 items".
    /// </summary>
    /// <param name="singularNoun"></param>
    /// <param name="pluralNoun"></param>
    /// <returns>A string describing the required quantity of items identified by the provided noun forms.</returns>
    public abstract string Describe(string singularNoun, string pluralNoun);

    private record ExactQuantity : Quantity
    {
        private readonly int _number;

        public ExactQuantity(int number)
        {
            _number = number;
        }

        public override bool Matches<T>(IEnumerable<T> items) { return _number == items.Count(); }
        public override bool RequiresMoreThan<T>(IEnumerable<T> items) { return _number > items.Count(); }
        public override string Describe(string singularNoun, string pluralNoun)
        {
            return string.Format("exactly {0} {1}", _number, _number == 1 ? singularNoun : pluralNoun);
        }
    }

    private record AnyNonZeroQuantity : Quantity
    {
        public override bool Matches<T>(IEnumerable<T> items) { return items.Any(); }
        public override bool RequiresMoreThan<T>(IEnumerable<T> items) { return !items.Any(); }

        public override string Describe(string singularNoun, string pluralNoun)
        {
            return string.Format("a {0}", singularNoun);
        }
    }

    private record NoneQuantity : Quantity
    {
        public override bool Matches<T>(IEnumerable<T> items) { return !items.Any(); }
        public override bool RequiresMoreThan<T>(IEnumerable<T> items) { return false; }
        public override string Describe(string singularNoun, string pluralNoun)
        {
            return "no " + pluralNoun;
        }
    }

    private record RangeQuantity : Quantity
    {
        private readonly int minInclusive;
        private readonly int maxInclusive;

        public RangeQuantity(int minInclusive, int maxInclusive)
        {
            if (minInclusive < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minInclusive),
                    $"{nameof(minInclusive)} must be >= 0, but was {minInclusive}.");
            }
            if (maxInclusive <= minInclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(maxInclusive),
                    $"{nameof(maxInclusive)} must be greater than {nameof(minInclusive)} (was {maxInclusive}, required > {minInclusive}).");
            }
            this.minInclusive = minInclusive;
            this.maxInclusive = maxInclusive;
        }
        public override string Describe(string singularNoun, string pluralNoun) =>
            $"between {minInclusive} and {maxInclusive} (inclusive) {((maxInclusive == 1) ? singularNoun : pluralNoun)}";

        public override bool Matches<T>(IEnumerable<T> items)
        {
            var count = items.Count();
            return count >= minInclusive && count <= maxInclusive;
        }

        public override bool RequiresMoreThan<T>(IEnumerable<T> items) => items.Count() < minInclusive;
    }
}