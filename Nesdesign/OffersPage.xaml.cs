using Nesdesign.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Nesdesign
{
    public partial class OffersPage : Page
    {
        OffersViewModel viewModel;
        private void addProject(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Offer offer)
            {
                offer.setProjectAndCopy(btn.Tag.ToString() == "construction");
                OffersDataGrid.Items.Refresh();
            }
        }

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Offer offer)
            {

                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                   offer.loadPhoto(openFileDialog.FileName);
                   OffersDataGrid.Items.Refresh();
                }
                
            }
        }


        public void CloseNewOrderForm()
        {
            Overlay.Visibility = Visibility.Collapsed;
        }

        public OffersPage(OffersViewModel offersViewModel)
        {
            InitializeComponent();
            this.DataContext = offersViewModel;
            this.viewModel = offersViewModel;

        }
   


        private void CreateOrderCLick(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button btn && btn.DataContext is Offer offer)
            {
            
                Overlay.Visibility = Visibility.Visible;
                FormPanel.LoadOrder(offer);
                FormPanel.SetCallback(result => OnFormResult(result));
                
       
                /*
                // MessageBoxResult result = MessageBox.Show($"Kliknięto A dla oferty: {offer.offerId}");

                string orderNumber = StringHandler.RandomString(10);
                    offer.orderNumber = orderNumber;
                    FileHandler.CreateDir(orderNumber, DIR_TYPE.Order);
                    OffersDataGrid.Items.Refresh();*/


                
            }
        }

        private void OnFormResult(Offer? offer)
        {
            Overlay.Visibility = Visibility.Collapsed;

            if (offer == null)
            {
     
                return;
            }

            OffersDataGrid.Items.Refresh();

            FileHandler.CreateDir(offer.orderPath, DIR_TYPE.Order);
            
            
   
        }


        private void OpenOrderFolderClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Offer offer)
            {
                string orderPath = offer.orderPath;
                FileHandler.OpenFolder(orderPath, DIR_TYPE.Order);
       
                OffersDataGrid.Items.Refresh();
               
            }
        }


    }
}
