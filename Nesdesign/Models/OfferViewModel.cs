using Nesdesign.Data;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

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

        // Wywołać z UI (np. MainWindow Loaded)
        public async Task InitializeAsync()
        {
            await SqliteDatabase.InitializeAsync();
            await LoadFromDatabaseAsync();
        }

        private async Task LoadFromDatabaseAsync()
        {
            var items = await SqliteDatabase.GetOffersAsync(); // <-- poprawione: GetOffersAsync zamiast SGetOffersAsync
            foreach (var o in items)
            {
                SubscribeOffer(o);
                Offers.Add(o);
            }
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
                // Fire-and-forget zapis asynchroniczny (bez blokowania UI). Obsługa wyjątków wewnątrz.
                _ = SaveOfferAsync(o);
            }
        }

        private static async Task SaveOfferAsync(Offer o)
        {
            try
            {
                await SqliteDatabase.InsertOrUpdateOfferAsync(o);
                Debug.WriteLine($"Offer {o.offerId} saved to database.");
            }
            catch (Exception)
            {
                // TODO: logowanie błędu
            }
        }

        private void Offers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Subscribe new items
            if (e.NewItems != null)
            {
                foreach (Offer o in e.NewItems)
                {
                    SubscribeOffer(o);
                    _ = SaveOfferAsync(o); // persist new item async
                }
            }

            // Unsubscribe removed items
            if (e.OldItems != null)
            {
                foreach (Offer o in e.OldItems)
                {
                    UnsubscribeOffer(o);
                    _ = SqliteDatabase.DeleteOfferAsync(o.offerId);
                }
            }

            // Notify LatestOfferId changes
            OnPropertyChanged(nameof(LatestOfferId));
            SelectedItem = Offers[^1];
            OnPropertyChanged(nameof(SelectedItem));

            // Refresh filter if applied
            OffersView.Refresh();
        }

        // Latest offerId property
        public string LatestOfferId => Offers.Count > 0 ? Offers[^1].offerId : "N0000000";
        public string SelectedOfferId => SelectedItem != null ? SelectedItem.offerId : "N0000000";

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
                return offer != null && offer.status == status;
            };
        }

        public void ClearFilter()
        {
            OffersView.Filter = null;
        }

        // -- DODANE: CountOffersMatching --
        // Zwraca liczbę ofert, których offerId (lub inne pole) pasuje do wzorca regex.
        // Przy błędnym wzorcu zwraca liczenie przez Contains (case-insensitive).
        public int CountOffersMatching(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;

            // snapshot kolekcji, by uniknąć problemów z enumeracją
            var snapshot = Offers.ToList();

            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                return snapshot.Count(o => !string.IsNullOrEmpty(o.offerId) && regex.IsMatch(o.offerId));
            }
            catch (ArgumentException)
            {
                // niepoprawny regex — fallback do Contains
                return snapshot.Count(o => !string.IsNullOrEmpty(o.offerId) && o.offerId.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        // INotifyPropertyChanged implementation

        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
