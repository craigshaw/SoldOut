using SoldOutSearchMonkey.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace SoldOutSearchMonkey
{
    class SoldOutSearchMonkey
    {
        private const string ServiceDescription = "SoldOut worker service. Scrapes auction results from eBay and logs them to a data store";
        private const string ServiceDisplayName = "SoldOut Search Monkey";
        private const string ServiceName = "SoldOutSearchMonkey";

        static void Main(string[] args)
        {
            new SoldOutSearchMonkey().Startup();
        }

        private void Startup()
        {
            HostFactory.Run(x =>
            {
                x.Service<SearchMonkeyService>(s =>
                {
                    s.ConstructUsing(name => new SearchMonkeyService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription(ServiceDescription);
                x.SetDisplayName(ServiceDisplayName);
                x.SetServiceName(ServiceName);
            });
        }
    }
}
