namespace Digital5HP.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;
    using FluentAssertions.Collections;
    using FluentAssertions.Execution;

    public static class AssertionExtensions
    {
        /// <summary>
        /// Asserts that a collection contains exactly a given number of elements, which each match respectively
        /// the predicate provided to the expected collection.
        /// </summary>
        /// <param name="assertions">The current assertion.</param>
        /// <param name="expected">
        /// The expected collection, which matches each element in turn. The
        /// total number of elements must exactly match the number of elements in the collection.
        /// </param>
        /// <param name="predicate">
        /// The predicate to match each respective elements from the in-test and expected collections.
        /// </param>
        /// <param name="because">
        /// An optional formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the
        /// assertion is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because" />.
        /// </param>
        public static AndConstraint<GenericCollectionAssertions<T>> MatchRespectively<T, TOther>(
            this GenericCollectionAssertions<T> assertions,
            IEnumerable<TOther> expected,
            Func<T, TOther, bool> predicate,
            string because = "",
            params object[] becauseArgs)
        {
            ArgumentNullException.ThrowIfNull(assertions);
            ArgumentNullException.ThrowIfNull(expected);
            ArgumentNullException.ThrowIfNull(predicate);

            var collection = expected.ToList();
            if (collection.Count == 0)
                throw new ArgumentException(
                    "Cannot verify against an empty expected collection",
                    nameof(expected));

            var num = assertions.Subject.Count();
            var count = collection.Count;

            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .ForCondition(num == count)
                   .FailWith(
                        "Expected {context:collection} to contain exactly {0} items{reason}, but it contains {1} items",
                        count,
                        num);

            for (var i = 0; i < num; i++)
            {
                var first = assertions.Subject.ElementAt(i);
                var second = collection[i];

                Execute.Assertion.BecauseOf(because, becauseArgs)
                       .ForCondition(predicate(first, second))
                       .FailWith(
                            "Expected {context:collection} to match all elements respectively, but some predicates failed.");
            }

            return new AndConstraint<GenericCollectionAssertions<T>>(assertions);
        }
    }
}
