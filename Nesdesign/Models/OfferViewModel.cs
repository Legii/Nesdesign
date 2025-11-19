using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Nesdesign.Models
{
    public class OffersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Offer> Offers { get; set; }

        private ICollectionView? _offersView;
        public ICollectionView OffersView
        {
            get
            {
                if (_offersView == null)
                    _offersView = CollectionViewSource.GetDefaultView(Offers);
                return _offersView;
            }
        }

        public OffersViewModel()
        {
            Offers = new ObservableCollection<Offer>();
            Offers.CollectionChanged += Offers_CollectionChanged;
        }


        public int CountOffersMatching(string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return Offers.Count(o => regex.IsMatch(o.offerId));   // or o.Customer or whatever field
        }
        private void Offers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Notify LatestOfferId changes
            OnPropertyChanged(nameof(LatestOfferId));

            // Refresh filter if applied
            OffersView.Refresh();
        }

        // Latest offerId property
        public string LatestOfferId => Offers.Count > 0 ? Offers[^1].offerId : "N0000000";
        public string Selected => LatestOfferId;
        public string SelectedProject;
        // Filtering methods
        public void FilterByStatus(OfferStatus status)
        {
            OffersView.Filter = obj =>
            {
                var offer = obj as Offer;
                return offer != null && offer.status == status;
            };
        }

        public void ClearFilter()
        {
            OffersView.Filter = null;

        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
