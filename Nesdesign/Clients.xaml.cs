using Nesdesign.Models;
using System;
using System.Collections.Generic;
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

namespace Nesdesign
{
    /// <summary>
    /// Logika interakcji dla klasy Klienci.xaml
    /// </summary>
    public partial class Clients : Page
    {
        private ClientsViewModel _viewModel;
        public Clients()
        {
            InitializeComponent();
            _viewModel = new ClientsViewModel();
            this.DataContext = _viewModel;

            this._viewModel.Clients.Add(new Client("676 234 15 99", "Twójstarex"));
            this._viewModel.Clients.Add(new Client("676 234 15 98", "Taaex"));
            this._viewModel.Clients.Add(new Client("676 234 15 97", "tosterx"));
            this._viewModel.Clients.Add(new Client("676 234 15 96", "Twójstarexxx"));
        }
    }
}
