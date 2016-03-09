using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoldOutBusiness.Models;
using System.Collections.Generic;
using SoldOutBusiness.Utilities.Collections;

namespace SoldOutTests
{
    [TestClass]
    public class LinqTreeTests
    {
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
