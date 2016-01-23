using log4net;
using SoldOutSearchMonkey.Service;
using System;
using Topshelf;

namespace SoldOutSearchMonkey
{
    class SoldOutSearchMonkey
    {
        private const string ServiceDescription = "SoldOut worker service. Scrapes auction results from eBay and logs them to a data store";
        private const string ServiceDisplayName = "SoldOut Search Monkey";
        private const string ServiceName = "SoldOutSearchMonkey";

        private static readonly ILog _log = LogManager.GetLogger(typeof(SoldOutSearchMonkey));

        static void Main(string[] args)
        {
            new SoldOutSearchMonkey().Startup();
        }

        private void Startup()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                _log.Fatal($"Unhandled exception thrown: {e.ExceptionObject}");
            };

            HostFactory.Run(config =>
            {
                config.Service<SearchMonkeyService>(service =>
                {
                    service.ConstructUsing(name => new SearchMonkeyService());
                    service.WhenStarted(svc => svc.Start());
                    service.WhenStopped(svc => svc.Stop());
                });
                config.RunAsLocalSystem();

                config.SetDescription(ServiceDescription);
                config.SetDisplayName(ServiceDisplayName);
                config.SetServiceName(ServiceName);
            });
        }
    }
}
