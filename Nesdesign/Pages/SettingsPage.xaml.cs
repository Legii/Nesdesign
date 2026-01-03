using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nesdesign.Pages
{

    public partial class SettingsPage : Page
    {
        private SettingsManager _settingsManager;

     

        public SettingsPage(SettingsManager manager)
        {
            InitializeComponent();
            DataContext = this;
            _settingsManager = manager;
            SettingsDataGrid.ItemsSource = _settingsManager.SettingsList;



        }

        public void SetupSettings(SettingsManager manager)
        {
;           _settingsManager = manager;
           



        }


    }
}
