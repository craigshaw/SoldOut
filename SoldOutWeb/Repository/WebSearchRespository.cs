using SoldOutBusiness.DAL;
using System;
using System.Diagnostics;
using SoldOutBusiness.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SoldOutWeb.Repository
{
    public class WebSearchRespository : IWebSearchRepository
    {
        public SoldOutContext _context;

        public SoldOutContext SoldOutContext { set { _context = value; } }

        public WebSearchRespository()
        {
            _context = new SoldOutContext();

#if DEBUG
            _context.Database.Log = s => Debug.WriteLine(s);
#endif
        }

        #region IDisposable Support
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        #endregion

        public IEnumerable<Product> SearchForProduct(string searchText)
        {
            return _context.Database.SqlQuery<Product>(
                "exec ProductSearchByManuFacturercodeAndName @SearchText",
                new SqlParameter("@SearchText", searchText));
        }
    }
}