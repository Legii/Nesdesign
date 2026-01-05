using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Windows;

namespace Nesdesign.Models
{



    public class ClientsViewModel : INotifyPropertyChanged
    {
        private bool loaded = false;
        public List<string> ClientNames => Clients.Select(c => c.Name).ToList();
        public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>();

        public ClientsViewModel()
        {
            Clients.CollectionChanged += Clients_CollectionChanged;
            LoadClientsAsync();
        }


        public async Task LoadClientsAsync()
        {
            var clientList = await DatabaseHandler.GetClientsAsync();
            foreach (var client in clientList)
            {
                Clients.Add(client);
                SubscribeClient(client);
            }
            loaded = true;
            if(Clients.Count == 0)
            {
          
                var defaultClient = new Client { Name = "-" };
                Clients.Add(defaultClient);
                await DatabaseHandler.AddRecordAsync(defaultClient);
            }
           /* string t = "";
            foreach (Client c in Clients)
            {
               t += c.Name +"("+c.ClientId + ")" + ", ";
            }
            MessageBox.Show("Clients loaded: " + t);*/
        }
        private void SubscribeClient(Client c)
        {
            if (c == null) return;
            c.PropertyChanged -= Client_PropertyChanged;
            c.PropertyChanged += Client_PropertyChanged;
        }

        private void UnsubscribeClient(Client c)
        {
            if (c == null) return;
            c.PropertyChanged -= Client_PropertyChanged;
        }

        private void Client_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is Client c)
            {
         
                DatabaseHandler.UpdateRecordAsync(c);
            }
        }

        private void Clients_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null)
            {
                foreach (Client c in e.NewItems)
                {
                    SubscribeClient(c);
                    if(loaded)
                        DatabaseHandler.AddRecordAsync(c);

                }
            }

            if (e.OldItems != null)
            {
                foreach (Client c in e.OldItems)
                {
                    UnsubscribeClient(c);
           
                }
            }

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(ClientNames));
         

        }



        public int Count => Clients.Count;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
