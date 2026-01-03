using Nesdesign.Models;
using System.Windows;
using System.Windows.Controls;

namespace Nesdesign
{
    public partial class ClientsPage : Page
    {
        private ClientsViewModel _viewModel;
        public ClientsPage(ClientsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;

        }
    }
}
