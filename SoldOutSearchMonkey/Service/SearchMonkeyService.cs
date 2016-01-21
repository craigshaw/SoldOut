using log4net;
using SoldOutBusiness.Repository;
using System;

namespace SoldOutSearchMonkey.Service
{
    public class SearchMonkeyService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SearchMonkeyService));
        //(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Start()
        {
            var repo = new SearchRepository();

            var searches = repo.GetAllSearches();

            foreach (var search in searches)
            {
                _log.Info(search.Name);
            }           
        }

        public void Stop()
        {
            _log.Info("Stopped");
        }
    }
}
