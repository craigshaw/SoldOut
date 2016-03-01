using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoldOutBusiness.Utilities
{
    public static class ConditionExtensions
    {
        public static Condition SingleOrDefault(this IEnumerable<Condition> source, Func<Condition, bool> predicate)
        {
            Condition condition = source.Where(predicate).SingleOrDefault();

            if (condition == null)
                condition = source.Where(c => c.Description == "Unknown").Single();

            return condition;
        }
    }
}
