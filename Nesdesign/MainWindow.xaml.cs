using Nesdesign.Models;
using Nesdesign.Pages;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
namespace Nesdesign
{
    public partial class MainWindow : Window
    {
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
            InitializeComponent();
            PreloadImages(new List<string> {  });
            settingsManager = new SettingsManager();
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SettingsManager.Instance.SaveToXml();
            base.OnClosing(e);
        }
    }
}