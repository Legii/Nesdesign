using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Nesdesign.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Nesdesign.MainWindow;

namespace Nesdesign
{
    public partial class OffersPage : Page, INotifyPropertyChanged
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
                    offer.LoadPhoto(openFileDialog.FileName);
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
            this.viewModel.PropertyChanged += (_, e) => { 
                if(e.PropertyName == nameof(OffersViewModel.ShowPaymentData))
                {
                    Visibility visibility = viewModel.ShowPaymentData ? Visibility.Visible : Visibility.Collapsed;
                    InvoiceColumn.Visibility = visibility;
                    PriceColumn.Visibility = visibility;
                    PaymentStatusColumn.Visibility = visibility;
                    LabelTotalSum.Visibility = visibility;
                    TextboxTotalSum.Visibility = visibility;

                }

  

            };
            WeakReferenceMessenger.Default.Register<RequestDeleteSelectedOfferMessage>(this, (r, m) =>
            {
          
                OffersDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                OffersDataGrid.CancelEdit();

                if (DataContext is OffersViewModel vm)
                    vm.DeleteSelected();
            });

        }
   

       


        private void CreateOrderCLick(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button btn && btn.DataContext is Offer offer)
            {
            
                Overlay.Visibility = Visibility.Visible;
                FormPanel.LoadOrder(offer);
                FormPanel.SetCallback(result => OnFormResult(result));
               
                
            }
        }
        private void RightClick(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt)
                Clipboard.SetText(txt?.Text?.ToString() ?? "");
            else if (sender is Button btn)
                Clipboard.SetText(btn.Content.ToString());
            else if (sender is DataGridCell cell && cell.Content is TextBlock tb)
                Clipboard.SetText(tb.Text.ToString());
        }

        private void OnFormResult(Offer? offer)
        {
            Overlay.Visibility = Visibility.Collapsed;

            if (offer == null)
            {
     
                return;
            }

            OffersDataGrid.Items.Refresh();

            FileHandler.CreateDir(offer.OrderPath, DIR_TYPE.Order);
            
            
   
        }


        private void OpenOrderFolderClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Offer offer)
            {
                string orderPath = offer.OrderPath;
        
                
                FileHandler.OpenFolder(orderPath, DIR_TYPE.Order);
       
               
            }
        }

        private void OpenProjectFolderClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Offer offer)
            {
                string projectPath = offer.projectPath;
                FileHandler.OpenFolder(projectPath, DIR_TYPE.Project);


            }
        }

        private void OpenOfferFolderClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Offer offer)
            {
                string offerPath = offer.OfferId;
                FileHandler.OpenFolder(offerPath, DIR_TYPE.Offer);


            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.ClearFilters();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                e.Handled = false; // allow newline
            }
            else if (e.Key == Key.Enter)
            {
                e.Handled = true; // prevent default DataGrid behavior
            }
        }

        // Handler pozwalający:
        // - odznaczyć wiersz klikając ponownie w zaznaczony wiersz
        // - wyczyścić zaznaczenie klikając w puste miejsce DataGrid
        private void OffersDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;

            var row = FindVisualParent<DataGridRow>(dep);

            if (row == null)
            {
                // kliknięcie w puste miejsce DataGrid => wyczyść zaznaczenie
                OffersDataGrid.UnselectAll();
                return;
            }

            // jeśli kliknięto już zaznaczony wiersz — odznacz go i przerwij dalsze przetwarzanie
            if (row.IsSelected)
            {
                OffersDataGrid.UnselectAll();
                e.Handled = true;
            }
            // w przeciwnym razie pozwól standardowemu zachowaniu zaznaczenia
        }

        // helper do przeszukiwania drzewa wizualnego w górę
        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child == null) return null;
            DependencyObject current = child;
            while (current != null)
            {
                if (current is T correctlyTyped)
                    return correctlyTyped;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        
    }
}
