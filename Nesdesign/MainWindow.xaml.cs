using CommunityToolkit.Mvvm.Messaging;
using Nesdesign.Models;
using Nesdesign.Pages;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
namespace Nesdesign
{
    public partial class MainWindow : Window

    {
        public static  MainWindow Instance { get; set; }

        private OffersFilter offersFilter;
        private DatabaseHandler dbHandler { get; } = new DatabaseHandler();
        private SettingsManager settingsManager;
        OffersViewModel offersViewModel { get; set; }
        public ClientsViewModel clientsViewModel { get; set; }
        ContractorsViewModel contractorsViewModel { get; set; }
        private OffersPage offersPage { get; set; }
    
        private ClientsPage clientsPage { get; set; }
        private CreatorPage creatorPage { get; set; }
        private SettingsPage settingsPage { get; set; }
        
        
        

        
        public void PreloadImages(IEnumerable<string> paths)
        {
            foreach (var p in paths)
                ImageHandler.Get(p);
        }

        public MainWindow()
        {
            contractorsViewModel = new ContractorsViewModel();
            settingsManager = new SettingsManager();
            Instance = this;

            
            
            FileHandler.CreateBaseDir();
            try
            {
                dbHandler.ConnectToDatabase();
            } catch
            {
                MessageBox.Show("Nie udało się utworzyć folderu lub połączyć z bazą danych"); return;
            }
          
            

            InitializeComponent();
            PreloadImages(new List<string> {  });
           
            clientsViewModel = new ClientsViewModel();
            offersViewModel = new OffersViewModel(clientsViewModel, contractorsViewModel);


            offersFilter = new OffersFilter(offersViewModel);
            clientsPage = new ClientsPage(clientsViewModel);
      
            creatorPage = new CreatorPage(offersViewModel , offersFilter);
            offersPage = new OffersPage(offersViewModel);
            settingsPage = new SettingsPage(settingsManager, contractorsViewModel);
           
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

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(settingsPage);
        }

        private void CreateOfferClick(object sender, RoutedEventArgs e)
        {
            creatorPage.CreateOfferAndCopy();
            MainFrame.Navigate(offersPage);
        }

        private void InProgressFilterClick(object sender, RoutedEventArgs e)
        {
            offersFilter.InProgressFilter(sender, e);
        }

        private void ReadyFilterClick(object sender, RoutedEventArgs e)
        {
           offersFilter.ReadyFilter(sender, e);
        }
        private void FinishedFilterClick(object sender, RoutedEventArgs e)
        {
           offersFilter.FinishedFilter(sender, e);
        }

        private void ClearFilterClick(object sender, RoutedEventArgs e)
        {
            offersFilter.ClearFilter(sender, e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SettingsManager.Instance.SaveToXml();
            base.OnClosing(e);
        }

        public class RequestDeleteSelectedOfferMessage { }

        private async void DeleteOfferButtn_Click(object sender, RoutedEventArgs e)
        {
            
            Offer offer = offersViewModel.SelectedItem as Offer;
            if(offer != null)
            {
                string id = offer.OfferId;
                var result = MessageBox.Show($"Czy usunać zapytanie {id}?", "Potwierdź akcję", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    WeakReferenceMessenger.Default.Send(new RequestDeleteSelectedOfferMessage());
 
              
                else
                    return;
            } else
            {
                MessageBox.Show("Nie wybrano żadnego zapytania do usunięcia");
            }
           
        }
    }
}