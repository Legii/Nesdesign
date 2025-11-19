using Nesdesign.Models;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OffersViewModel offersViewModel { get; set; }
        public OffersPage offersPage { get; set; }
        public Clients clientsPage { get; set; }
        public Creator creatorPage { get; set; }
        public MainWindow()
        {

            InitializeComponent();
            offersViewModel = new OffersViewModel();
            clientsPage = new Clients();
            creatorPage = new Creator(offersViewModel);
            offersPage = new OffersPage(offersViewModel);
            MainFrame.Navigate(offersPage);
        }

        public void NavigateToOffersPage()
        {
            MainFrame.Navigate(offersPage);
        }

        private void OffersClick(object sender, RoutedEventArgs e)
        {
            NavigateToOffersPage();
        }
        private void ClientsClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(clientsPage);
        }

        private void CreatorClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(creatorPage);
        }
    }
}