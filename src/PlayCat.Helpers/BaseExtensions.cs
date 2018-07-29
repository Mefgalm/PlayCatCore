using System;
using System.Collections.Generic;
using System.Linq;


namespace PlayCat.Helpers
{
    public static class BaseExtensions
    {

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            return !enumerable.Any();
        }
    }
}
