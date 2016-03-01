using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutBusiness.Models;
using System.Collections.Generic;
using SoldOutBusiness.Utilities;

namespace SoldOutTests
{
    [TestClass]
    public class ConditionExtensionTests
    {
        [TestMethod]
        public void ConditionSingleOrDefaultFindsCondition()
        {
            IList<Condition> conditions = CreateTestConditions();

            var result = conditions.SingleOrDefault(c => c.eBayConditionId == 1000);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Description, "New");
        }

        [TestMethod]
        public void ConditionSingleOrDefaultDefaultsToUnknown()
        {
            IList<Condition> conditions = CreateTestConditions();

            var result = conditions.SingleOrDefault(c => c.eBayConditionId == 2999);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Description, "Unknown");
        }

        private IList<Condition> CreateTestConditions()
        {
            return new List<Condition>() {
                new Condition() { ConditionId = 1, Description = "New", eBayConditionId = 1000 },
                new Condition() { ConditionId = 2, Description = "Used", eBayConditionId = 2000 },
                new Condition() { ConditionId = 3, Description = "Unknown", eBayConditionId = 0 }
            };
        }
    }
}
