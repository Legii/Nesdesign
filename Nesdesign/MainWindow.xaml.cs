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
        private DatabaseHandler dbHandler { get; } = new DatabaseHandler();
        private SettingsManager settingsManager;
        OffersViewModel offersViewModel { get; set; }
        ClientsViewModel clientsViewModel { get; set; }
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
            settingsManager = new SettingsManager();
            
            FileHandler.CreateBaseDir();
            try
            {
                dbHandler.ConnectToDatabase();
            } catch
            {
                MessageBox.Show("Nie udało się utworzyć folderu lub połączzyć z bazą danych"); return;
            }
          
            

            InitializeComponent();
            PreloadImages(new List<string> {  });
           
            clientsViewModel = new ClientsViewModel();
            offersViewModel = new OffersViewModel(clientsViewModel);
          

            clientsPage = new ClientsPage(clientsViewModel);
      
            creatorPage = new CreatorPage(offersViewModel);
            offersPage = new OffersPage(offersViewModel);
            settingsPage = new SettingsPage(settingsManager);
           
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
                var result = MessageBox.Show($"Czy usunać ofertę {id}?", "Potwierdź akcję", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    WeakReferenceMessenger.Default.Send(new RequestDeleteSelectedOfferMessage());
 
              
                else
                    return;
            } else
            {
                MessageBox.Show("Nie wybrano żadnej oferty do usuniecia");
            }
           
        }
    }
}