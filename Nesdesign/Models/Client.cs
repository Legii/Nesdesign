using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nesdesign.Models
{
    public class Client : INotifyPropertyChanged
    {
        private int _clientId;
        public int clientId
        {
            get => _clientId;
            set
            {
                if (_clientId == value) return;
                _clientId = value;
                OnPropertyChanged(nameof(clientId));
            }
        }

        private string? _nazwa;
        public string? nazwa
        {
            get => _nazwa;
            set
            {
                if (_nazwa == value) return;
                _nazwa = value;
                OnPropertyChanged(nameof(nazwa));
            }
        }

        private string? _nip;
        public string? NIP
        {
            get => _nip;
            set
            {
                if (_nip == value) return;
                _nip = value;
                OnPropertyChanged(nameof(NIP));
            }
        }

        private string? _opis;
        public string? opis
        {
            get => _opis;
            set
            {
                if (_opis == value) return;
                _opis = value;
                OnPropertyChanged(nameof(opis));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
    }
}
