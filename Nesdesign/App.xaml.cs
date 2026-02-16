using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Nesdesign
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
       
        protected override void OnStartup(StartupEventArgs e)
        {
            Console.WriteLine("Start;");
            base.OnStartup(e);

        }
    }

}
