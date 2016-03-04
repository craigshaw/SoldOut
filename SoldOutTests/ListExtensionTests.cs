using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutBusiness.Utilities.Collections;
using System.Collections.Generic;

namespace SoldOutTests
{
    [TestClass]
	public class ListExtensionTests
	{
		[TestMethod]
		public void ReturnsTrueIfNull()
		{
            List<int> nullList = null;

            Assert.IsTrue(nullList.IsNullOrEmpty());
		}

        [TestMethod]
        public void ReturnsTrueIfEmpty()
        {
            List<int> emptyList = new List<int>();

            Assert.IsTrue(emptyList.IsNullOrEmpty());
        }

        [TestMethod]
        public void ReturnsFalseIfNotNullOrEmpty()
        {
            List<string> list = new List<string>() { "Test" };

            Assert.IsFalse(list.IsNullOrEmpty());
        }
    }
}
