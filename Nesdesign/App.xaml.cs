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
        public static DatabaseHandler dbHandler { get; } = new DatabaseHandler();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            dbHandler.ConnectToDatabase();
        }
    }

}
