using Nesdesign.Models;
using System.Windows;
using System.Windows.Controls;

namespace Nesdesign
{
    public partial class Clients : Page
    {
        private ClientsViewModel _viewModel;
        public Clients()
        {
            InitializeComponent();
            _viewModel = new ClientsViewModel();
            this.DataContext = _viewModel;

            // ładuj asynchronicznie dane z bazy
            _ = _viewModel.InitializeAsync();
            // opcjonalnie: zarejestruj flush przy zamykaniu okna (jeśli potrzebne)
        }
    }
}
