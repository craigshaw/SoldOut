using log4net;
using MahApps.Metro;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace SoldOutCleanser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Application));

        protected override void OnStartup(StartupEventArgs e)
        {
            // http://stackoverflow.com/questions/2764615/wpf-stringformat-0c-showing-as-dollars
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.IetfLanguageTag)));

            // Set theme
            //ThemeManager.ChangeAppStyle(Application.Current,
            //                            ThemeManager.GetAccent("Purple"),
            //                            ThemeManager.GetAppTheme("BaseDark"));

            // Register for global exception handling
            AppDomain.CurrentDomain.UnhandledException += (o, ex) =>
            {
                _log.Fatal($"Unhandled exception thrown: {ex.ExceptionObject}");
            };

            base.OnStartup(e);
        }
    }
}
