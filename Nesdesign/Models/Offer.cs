using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nesdesign.Models
{
    public enum OfferStatus {
        [System.ComponentModel.Description("Utworzona")]
        UTWORZONA,
        [System.ComponentModel.Description("Nie ofertowana")]
        NIE_OFERTOWANA,
        [System.ComponentModel.Description("Oferta mailowa")]
        OFERTA_MAILOWA,
        [System.ComponentModel.Description("Oferta")]
        OFERTA,
        [System.ComponentModel.Description("Zamówienie")]
        ZAMOWIENIE,
        [System.ComponentModel.Description("W realizacji")]
        W_REALIZACJI,
        [System.ComponentModel.Description("Gotowa")]
        GOTOWA,
        [System.ComponentModel.Description("Wysłana")]
        WYSLANA,
        [System.ComponentModel.Description("Zakończona")]
        ZAKONCZONA
    }

    public class Offer : INotifyPropertyChanged
    {
        public string offerId { get; set; }
        public string fotoUrl { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public string name { get; set; }
        public int clientId { get; set; }

        private OfferStatus _status;
        public OfferStatus status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged(nameof(status));
                OnPropertyChanged(nameof(statusString));
            }
        }

        public string statusString => StringHandler.GetEnumString(this.status);

        private bool _closed;
        public bool Closed
        {
            get => _closed;
            set
            {
                if (_closed == value) return;
                _closed = value;
                OnPropertyChanged(nameof(Closed));
            }
        }

        public Offer() { }

        public Offer(string offerId, string fotoUrl, string description, int quantity, string name, int clientId)
        {
            this.offerId = offerId;
            this.fotoUrl = fotoUrl;
            this.description = description;
            this.quantity = quantity;
            this.name = name;
            this.clientId = clientId;
            this.status = OfferStatus.UTWORZONA;
            Closed = false;
        }

        public static Offer TemplateConstructor()
        {
            return new Offer
            {
                offerId = "N00" + StringHandler.RandomString(4),
                fotoUrl = "fotoUrl",
                description = StringHandler.RandomString(20),
                quantity = 0,
                name = StringHandler.RandomString(7),
                clientId = 0,
      
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
    }
}
