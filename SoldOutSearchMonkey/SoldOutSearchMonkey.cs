using log4net;
using SoldOutBusiness.Services;
using SoldOutBusiness.Services.Notifiers.Slack;
using SoldOutSearchMonkey.Service;
using System;
using System.Configuration;
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
                    service.ConstructUsing(name =>
                    {
                        var notifier = new SlackNotifier(ConfigurationManager.AppSettings["SlackServiceUri"]);

                        var finder = new EbayFinder()
                                        .Configure(c =>
                                        {
                                            // Initialize service end-point configuration
                                            c.EndPointAddress = "http://svcs.ebay.com/services/search/FindingService/v1";
                                            c.GlobalId = "EBAY-GB";
                                            // set eBay developer account AppID here!
                                            c.ApplicationId = ConfigurationManager.AppSettings["eBayApplicationId"];
                                        });

                        return new SearchMonkeyService(finder, notifier);
                    });
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
