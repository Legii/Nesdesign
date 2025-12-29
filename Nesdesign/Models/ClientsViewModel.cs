using Nesdesign.Data;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nesdesign.Models
{
    public class ClientsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Client> Clients { get; set; } = new();

        private readonly object _lockPending = new();
        private readonly System.Collections.Generic.List<Task> _pendingSaves = new();

        public ClientsViewModel()
        {
            Clients.CollectionChanged += Clients_CollectionChanged;
        }

        public async Task InitializeAsync()
        {
            await SqliteDatabase.InitializeAsync();
            await LoadFromDatabaseAsync();
        }

        private async Task LoadFromDatabaseAsync()
        {
            var items = await SqliteDatabase.GetClientsAsync();
            foreach (var c in items)
            {
                SubscribeClient(c);
                Clients.Add(c);
            }
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
                var t = SqliteDatabase.InsertOrUpdateClientAsync(c);
                RegisterPendingSave(t);
            }
        }

        private void Clients_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Client c in e.NewItems)
                {
                    SubscribeClient(c);
                    var t = SqliteDatabase.InsertOrUpdateClientAsync(c);
                    RegisterPendingSave(t);
                }
            }

            if (e.OldItems != null)
            {
                foreach (Client c in e.OldItems)
                {
                    UnsubscribeClient(c);
                    // opcjonalnie usuń z bazy: (implementacja DeleteClientAsync nie dodana)
                }
            }

            OnPropertyChanged(nameof(Count));
        }

        private void RegisterPendingSave(Task t)
        {
            lock (_lockPending)
            {
                _pendingSaves.Add(t);
            }
            _ = t.ContinueWith(task =>
            {
                lock (_lockPending)
                {
                    _pendingSaves.Remove(task);
                }
                if (task.IsFaulted)
                    Debug.WriteLine($"[ClientsViewModel] Save failed: {task.Exception}");
            }, TaskScheduler.Default);
        }

        public async Task FlushAsync()
        {
            Task[] tasks;
            lock (_lockPending) tasks = _pendingSaves.ToArray();
            if (tasks.Length == 0) return;
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ClientsViewModel] FlushAsync error: {ex}");
            }
        }

        public int Count => Clients.Count;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
