using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutBusiness.Utilities.Collections;
using System.Collections.Generic;
using SoldOutBusiness.Models;
using System.Linq;

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
        public void TreeRecursionReturnsCompleteTree()
        {
            var category = new List<Category>() {
                new Category() { Name = "Lego", CategoryID = 1, Children = new List<Category>()
                    {
                        new Category() { Name = "Star Wars", ParentCategoryId = 1, CategoryID = 2 }
                    }
                }
            };

            var result = category.SelectNestedChildren(c => c.Children).ToList();

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].Name,"Lego");
            Assert.AreEqual(result[1].Name,"Star Wars");
        }

        [TestMethod]
        public void TreeRecursionReturnsChildrenForSelectedParent()
        {
            var category = new List<Category>() {
                new Category() { Name = "Lego", CategoryID = 1, Children = new List<Category>()
                    {
                        new Category() { Name = "Star Wars", ParentCategoryId = 1, CategoryID = 2 }
                    },               
                },
                 new Category() { Name = "Retro Games", CategoryID = 3 }
            };

            var result = category.SelectNestedChildren(c => c.Children).ToList()
                        .Where(c => c.ParentCategoryId == 1).ToList();

            Assert.AreSame(category[0].Children.ToList()[0], result[0]);
        }
    }
}
