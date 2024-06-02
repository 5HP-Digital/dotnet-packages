namespace Digital5HP.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        /// <summary>
        /// Gets all defined values for a given Enum for use
        /// by <see cref="Xunit.MemberDataAttribute"/>
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T">The type of Enum</typeparam>
        public static IEnumerable<object[]> AsMemberData<T>(this IEnumerable<T> source)
            where T : Enum
        {
            return source.Select(value => new object[] {value});
        }
    }
}