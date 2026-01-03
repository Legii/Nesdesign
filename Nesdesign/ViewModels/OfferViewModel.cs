
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Nesdesign.Models
{
    public class OffersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Offer> Offers { get; set; }
        public ClientsViewModel ClientsModel { get; set; }

        private ICollectionView? _offersView;
        private bool loaded = false;

        private bool showPaymentData = true;

        public bool ShowPaymentData
        {
            get => showPaymentData;
            set
            {
                if (showPaymentData != value)
                {
                    showPaymentData = value;
                    
                    OnPropertyChanged(nameof(ShowPaymentData));
                }
            }
        }

        public ICollectionView OffersView
        {
            get
            {
                if (_offersView == null)
                    _offersView = CollectionViewSource.GetDefaultView(Offers);
               
                return _offersView;
            }
        }

        public OffersViewModel(ClientsViewModel clientsModel)
        {
            Offers = new ObservableCollection<Offer>();
            ClientsModel = clientsModel;
            Offers.CollectionChanged += Offers_CollectionChanged;
            LoadOffersAsync();
        
       
          

        }

        public async Task LoadOffersAsync()
        {
            var offersList = await DatabaseHandler.GetOffersAsync();
            foreach (var offer in offersList)
            {
                Offers.Add(offer);
                offer.LoadPhoto(offer.PhotoPath);
                SubscribeOffer(offer);
            }
            loaded = true;
        }

    


        private void SubscribeOffer(Offer o)
        {
            if (o == null) return;
            o.PropertyChanged -= Offer_PropertyChanged;
            o.PropertyChanged += Offer_PropertyChanged;
        }

        private void UnsubscribeOffer(Offer o)
        {
            if (o == null) return;
            o.PropertyChanged -= Offer_PropertyChanged;
        }

        private void Offer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {

            if (sender is Offer o)
            {
               
                DatabaseHandler.UpdateRecordAsync(o);

            }

        }

        private async void Offers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
               
                foreach (Offer o in e.NewItems)
                {
                    SubscribeOffer(o);
                    if (loaded)
                        await DatabaseHandler.AddRecordAsync(o);

                }
            }

            // Unsubscribe removed items
            if (e.OldItems != null)
            {
                foreach (Offer o in e.OldItems)
                {
                    UnsubscribeOffer(o);
                    //_ = SqliteDatabase.DeleteOfferAsync(o.OfferId);
                }
            }

            // Notify LatestOfferId changes
            OnPropertyChanged(nameof(LatestOfferId));
            SelectedItem = Offers[^1];
            OnPropertyChanged(nameof(SelectedItem));

            // Refresh filter if applied
            OffersView.Refresh();
        }

 
        // Latest OfferId property
        public string LatestOfferId => Offers.Count > 0 ? Offers[^1].OfferId : "N0000000";
        public string SelectedOfferId => SelectedItem != null ? SelectedItem.OfferId : "N0000000";

        private Offer _selectedItem;
        public Offer SelectedItem
        {
            get => _selectedItem;
            set
            {
                // Prevent null selection
                if (value == null && Offers.Count > 0)
                    value = Offers[0];

                _selectedItem = value;
            
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(SelectedOfferId));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public string SelectedProject;
        // Filtering methods
        public void FilterByStatus(OfferStatus status)
        {
            OffersView.Filter = obj =>
            {
                var offer = obj as Offer;
                return offer != null && offer.Status == status;
            };
        }

        public void FilterByPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                ClearFilter();
                return;
            }
            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                OffersView.Filter = obj =>
                {
                    var offer = obj as Offer;
                    return offer != null && !string.IsNullOrEmpty(offer.OfferId) && regex.IsMatch(offer.OfferId);
                };
            }
            catch (ArgumentException)
            {
                OffersView.Filter = obj =>
                {
                    var offer = obj as Offer;
                    return offer != null && !string.IsNullOrEmpty(offer.OfferId) && offer.OfferId.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;
                };
            }
        }

        public void ClearFilter()
        {
            OffersView.Filter = null;
        }


        public int CountOffersMatching(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;

            var snapshot = Offers.ToList();

            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                return snapshot.Count(o => !string.IsNullOrEmpty(o.OfferId) && regex.IsMatch(o.OfferId));
            }
            catch (ArgumentException)
            {
                return snapshot.Count(o => !string.IsNullOrEmpty(o.OfferId) && o.OfferId.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
