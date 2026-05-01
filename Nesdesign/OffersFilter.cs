using Nesdesign.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Nesdesign.Pages
{
    
    public class OffersFilter
    {

     
        private OffersViewModel _viewModel;

        public OffersFilter(OffersViewModel viewModel)
        {
            this._viewModel = viewModel;
        }
        private void Filter(OfferStatus status)
        {
            this._viewModel.FilterByStatus(status);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

 


        public void CreatedFilter(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.UTWORZONA);

        }
        public void Option2Filter(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.OFERTA);

        }

        public void Option3Filter(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.NIE_OFERTOWANA);

        }

        public void InProgressFilter(object sender, RoutedEventArgs e)
        {
            List<OfferStatus> t = new List<OfferStatus>();
            t.Add(OfferStatus.ZAMOWIENIE);
            t.Add(OfferStatus.W_REALIZACJI);
            t.Add(OfferStatus.GOTOWA);
            t.Add(OfferStatus.CZESCIOWE_ZAMOWIENIE);
            t.Add(OfferStatus.W_PRODUKCJI);

            this._viewModel.FIlterByMultipleStatuses(t);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

        public void ReadyFilter(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.GOTOWA);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }

        }
        public void FinishedFilter(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.ZAKONCZONA);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

        public void ClearFilter(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilters();
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }
    }
}
