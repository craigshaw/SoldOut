using MahApps.Metro;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace SoldOut
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // http://stackoverflow.com/questions/2764615/wpf-stringformat-0c-showing-as-dollars
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.IetfLanguageTag)));

            // Set theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent("Purple"),
                                        ThemeManager.GetAppTheme("BaseDark"));

            base.OnStartup(e);
        }
    }
}
