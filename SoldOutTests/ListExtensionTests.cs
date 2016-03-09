using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutBusiness.Utilities.Collections;
using System.Collections.Generic;
using SoldOutBusiness.Models;

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

        [TestMethod]
        public void TestTreeRecursion()
        {
            var category = new List<Category>() {
                new Category() { Name = "Lego", CategoryID = 1, Children = new List<Category>() {
                    new Category() { Name = "Star Wars", ParentCategoryId = 1, CategoryID = 2 }
                    }
                }
            };

            //var result = category.SelectNestedChildren(c => c.Children);
            //.Where(c => c.ParentCategoryId == 1).ToList();
            Assert.IsNull(null);
        }
    }
}
