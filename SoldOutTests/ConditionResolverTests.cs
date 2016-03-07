using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutBusiness.Models;
using System.Collections.Generic;
using SoldOutBusiness.Utilities.Conditions;
using System;

namespace SoldOutTests
{
    [TestClass]
    public class ConditionResolverTests
    {
        [TestMethod]
        public void ResolvesEBayConditionIdToConditionId()
        {
            var resolver = new ConditionResolver(CreateTestConditions());

            int actual = resolver.ConditionIdFromEBayConditionId(1000);

            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public void ResolvesUnknownEBayConditionIdToUnknownConditionId()
        {
            var resolver = new ConditionResolver(CreateTestConditions());

            int actual = resolver.ConditionIdFromEBayConditionId(10);

            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsIfConstructedWithNullConditionList()
        {
            var resolver = new ConditionResolver(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsIfConstructedWithEmptyConditionList()
        {
            var resolver = new ConditionResolver(new List<Condition>());
        }

        [TestMethod]
        public void ResolvesDescriptionFromConditionId()
        {
            var resolver = new ConditionResolver(CreateTestConditions());
            Assert.AreEqual("New", resolver.ConditionDescriptionFromConditionId(2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ResolveDescriptionFromConditionIdThrowsIfConditionIdNotKnown()
        {
            var resolver = new ConditionResolver(CreateTestConditions());
            resolver.ConditionDescriptionFromConditionId(6);
        }

        private IList<Condition> CreateTestConditions()
        {
            return new List<Condition>()
            {
                new Condition() { ConditionId = 1, Description = "Unknown", eBayConditionId = 0 },
                new Condition() { ConditionId = 2, Description = "New", eBayConditionId = 1000 },
                new Condition() { ConditionId = 7, Description = "Used", eBayConditionId = 3000 }
            };
        }
    }
}
