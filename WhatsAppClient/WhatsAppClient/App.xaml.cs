using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WhatsAppClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            if (!File.Exists("state.log"))
                File.WriteAllText("state.log", "");

            string[] infos = File.ReadAllLines("state.log");

            if (infos.Length == 0)
                StartupUri = new Uri("MVVM\\Views\\PolicyScreen.xaml", UriKind.Relative);
            else if (infos.Length == 1)
                StartupUri = new Uri("MVVM\\Views\\LoginScreen.xaml", UriKind.Relative);
            else if (infos.Length == 2)
                StartupUri = new Uri("MVVM\\Views\\InfoScreen.xaml", UriKind.Relative);
            else if (infos.Length == 3)
                StartupUri = new Uri("MVVM\\Views\\AppScreen.xaml", UriKind.Relative);

            base.OnStartup(e);
        }
    }
}
