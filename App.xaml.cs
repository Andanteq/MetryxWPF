using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Threading;

namespace MetryxWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        CultureInfo culture = new CultureInfo("ru-RU");

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}

