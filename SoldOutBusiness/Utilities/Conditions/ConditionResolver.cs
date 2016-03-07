using SoldOutBusiness.Models;
using SoldOutBusiness.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoldOutBusiness.Utilities.Conditions
{
    public class ConditionResolver : IConditionResolver
    {
        private readonly IList<Condition> _conditions;

        public ConditionResolver(IList<Condition> conditions)
        {
            if (conditions.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(conditions));

            _conditions = conditions;
        }

        public int ConditionIdFromEBayConditionId(int eBayConditionId)
        {
            return _conditions.SingleOrDefault(c => c.eBayConditionId == eBayConditionId).ConditionId;
        }

        public string ConditionDescriptionFromConditionId(int conditionId)
        {
            return _conditions.Single(c => c.ConditionId == conditionId).Description;
        }
    }
}
