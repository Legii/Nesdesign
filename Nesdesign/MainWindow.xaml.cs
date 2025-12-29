using Nesdesign.Models;
using System.Collections;
using System.Windows;

namespace Nesdesign
{
    public partial class MainWindow : Window
    {
        OffersViewModel offersViewModel { get; set; }
        public OffersPage offersPage { get; set; }
        public Clients clientsPage { get; set; }
        public Creator creatorPage { get; set; }
        

        public void PreloadImages(IEnumerable<string> paths)
        {
            foreach (var p in paths)
                ImageCache.Get(p);
        }

        public MainWindow()
        {
            InitializeComponent();
            //PreloadImages(new List<string> {  });
            offersViewModel = new OffersViewModel();
            clientsPage = new Clients();
            creatorPage = new Creator(offersViewModel);
            offersPage = new OffersPage(offersViewModel);
            MainFrame.Navigate(offersPage);
            
            this.Loaded += MainWindow_Loaded; // uruchom inicjalizację bazy i ładowanie asynchronicznie po załadowaniu okna
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicjalizacja i ładowanie danych (nie blokuje UI)
            await offersViewModel.InitializeAsync();
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