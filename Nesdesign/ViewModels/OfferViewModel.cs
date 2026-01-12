

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
        public ContractorsViewModel ContractorsModel { get; set; }

        private string _filterPattern { get; set; } = string.Empty;
        public string FilterPattern
        {
            get => _filterPattern;
            set
            {
                _filterPattern = value;
                ApplyCombinedFilter(FilterPattern);
                OnPropertyChanged(nameof(FilterPattern));
            }
        }



        private int? _filteredClientId;
        public int? FilteredClientId
        {
            get => _filteredClientId;
            set
            {
               
                _filteredClientId = value;

                ApplyCombinedFilter(FilterPattern);
                OnPropertyChanged(nameof(FilteredClientId));
            }
        }

        private OfferStatus? _filteredStatus;
        public OfferStatus? FilteredStatus
        {
            get => _filteredStatus;
            set
            {
                _filteredStatus = value;
                ApplyCombinedFilter(FilterPattern);
                OnPropertyChanged(nameof(FilteredStatus));
            }
        }


        private ICollectionView? _offersView;
        private bool loaded = false;

        private bool showPaymentData = true;
        private decimal _totalSum;

        public decimal TotalSum
        {
            get => _totalSum;
            private set
            {
                _totalSum = value;
                OnPropertyChanged(nameof(TotalSum));
            }
        }

        private void UpdateSum()
        {
        TotalSum = OffersView
        .Cast<Offer>()
        .Sum(o => o.Price != null ? (decimal)o.Price : 0);
        
        }


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

        public OffersViewModel(ClientsViewModel clientsModel, ContractorsViewModel contractorsModel)
        {
            Offers = new ObservableCollection<Offer>();
            ClientsModel = clientsModel;
            ContractorsModel = contractorsModel;
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

             
                if (e.PropertyName != nameof(Offer.AllInfo) && e.PropertyName != nameof(Offer.Photo))
                {
                   o.UpdateAllInfo();
                }
                UpdateSum();
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
                        _ = DatabaseHandler.AddRecordAsync(o);

                }
            }

            // Unsubscribe removed items
            if (e.OldItems != null)
            {
               

                foreach (Offer o in e.OldItems)
                {
                    UnsubscribeOffer(o);
                    _ = DatabaseHandler.DeleteRecordAsync(o);
                }
            }

           

            if (Offers.Count > 0)
                SelectedItem = Offers[^1];
            else
                SelectedItem = null;
           UpdateSum();

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

                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(SelectedOfferId));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public string SelectedProject;
        // Filtering methods
 


        public bool StatusFilter(object obj)
        {
            if (obj is Offer offer)
            {
                if (FilteredStatus.HasValue)
                {
                    return offer.Status == FilteredStatus.Value;
                }
                return true; // No status filter applied
            }
            return false;
        }

        public bool ClientFilter(object obj)
        {
            if (obj is Offer offer)
            {
                if (FilteredClientId.HasValue)
                {
                    return offer.ClientId == FilteredClientId.Value;
                }
                return true; // No client filter applied
            }
            return false;
        }

        public void DeleteSelected()
        {

            if (SelectedItem == null)
                return;
           var toDelete = SelectedItem;
            SelectedItem = null;
            Offers.Remove(toDelete);

        }

        public bool PatternFilter(object obj, string pattern)
        {
            if (obj is Offer offer)
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    return true; // No pattern filter applied
                }
                try
                {
                    var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    return !string.IsNullOrEmpty(offer.AllInfo) && regex.IsMatch(offer.AllInfo);
                }
                catch (ArgumentException)
                {
                    return !string.IsNullOrEmpty(offer.AllInfo) && offer.AllInfo.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            return false;
        }

        public void ApplyCombinedFilter(string pattern)
        {
            OffersView.Filter = obj =>
            {
                return StatusFilter(obj) && ClientFilter(obj) && PatternFilter(obj, pattern);
            };
            UpdateSum();
        }


        
        public void FilterByStatus(OfferStatus status)
        {
            ClearFilters();
            FilteredStatus = status;
            OffersView.Filter = obj =>
            {
                var offer = obj as Offer;
                return offer != null && offer.Status == status;
            };
        }

        public void ClearFilters()
        {
            if(FilteredStatus.HasValue || FilteredStatus.HasValue || FilterPattern != "")
            {
                FilteredStatus = null;
                FilteredClientId = null;
                FilterPattern = string.Empty;
            }
   

            OffersView.Filter = null;
            UpdateSum();
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

        public void RemoveOffer(Offer offer)
        {
            this.Offers.Remove(offer);
        }
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
