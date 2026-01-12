using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;

namespace Nesdesign.Models
{
    public class ContractorsViewModel : INotifyPropertyChanged
    {
     

        // Kolekcja edytowana w UI
        public ObservableCollection<Who> Contractors { get; set; } = new ObservableCollection<Who>();
        public List<string> ContractorNames => Contractors.Select(c => c.Name).ToList();

        // Komenda do zapisu
        public ICommand SaveCommand { get; }

        public ContractorsViewModel()
        {
            Contractors.CollectionChanged += Contractors_CollectionChanged;

            //LoadContractorsAsync();
        }

        // Wczytywanie danych z bazy
        public async Task LoadContractorsAsync()
        {
            var list = await DatabaseHandler.GetContractorsAsync();

            foreach (var c in list)
            {
                Contractors.Add(c);
                SubscribeContractor(c);
            } 
            if (Contractors.Count == 0)
            {
                var defaultContractor = new Who { Name = "-" };
                Contractors.Add(defaultContractor);
                await DatabaseHandler.AddRecordAsync(defaultContractor);
            }


        }

        // Subskrypcja zmian właściwości — ale NIC nie zapisujemy automatycznie
        private void SubscribeContractor(Who c)
        {
            if (c == null) return;
            c.PropertyChanged -= Contractor_PropertyChanged;
            c.PropertyChanged += Contractor_PropertyChanged;
        }

        private void UnsubscribeContractor(Who c)
        {
            if (c == null) return;
            c.PropertyChanged -= Contractor_PropertyChanged;
        }

        private void Contractor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ContractorNames));
        }


        private void Contractors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Who c in e.NewItems)
                    SubscribeContractor(c);
            }

            if (e.OldItems != null)
            {
                foreach (Who c in e.OldItems)
                    UnsubscribeContractor(c);
            }

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(ContractorNames));
        }

        // Zapis wszystkich zmian do bazy
        public async Task SaveAllAsync()
        {
            // Najprostsza wersja: nadpisujemy wszystkie rekordy
            foreach (var c in Contractors)
                await DatabaseHandler.UpdateRecordAsync(c);
        }

        public int Count => Contractors.Count;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}