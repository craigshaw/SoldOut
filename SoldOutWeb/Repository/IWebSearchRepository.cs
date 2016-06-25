using SoldOutBusiness.Models;
using System;
using System.Collections.Generic;

namespace SoldOutWeb.Repository
{
    internal interface IWebSearchRepository : IDisposable
    {
        IEnumerable<Product> SearchForProduct(string searchText);
    }
}
