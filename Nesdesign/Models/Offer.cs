using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Nesdesign.Models
{
    public enum OfferStatus {
        [System.ComponentModel.Description("Utworzona")]
        UTWORZONA,
        [System.ComponentModel.Description("nie ofertowana")]
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
        public string photoPath { get; set; }
        public ImageSource Photo { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public string name { get; set; }
        public int clientId { get; set; }
        private string _orderNumber;
        public string orderNumber
        {
            get => _orderNumber;
            set
            {
                _orderNumber = value;
                OnPropertyChanged(nameof(orderNumber));
            }
        }
        public string orderPath;

        public string construction { get; set; }
        public string production { get; set; }
        public string who { get; set; }
        public DateOnly date1 { get; set; }
        public DateOnly date2 { get; set; }
        public string invoiceNumber { get; set; }
        public int price { get; set; }


        
        public DateOnly sentDate { get; set; }

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

        private bool _closed = false;
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

        public string projectPath => construction != null ? construction : production;

        public Offer() {}

        public Offer(string offerId, ImageSource imageSource, string description, int quantity, string name, int clientId)
        {
            this.offerId = offerId;
            this.Photo = imageSource;
            this.description = description;
            this.quantity = quantity;
            this.name = name;
            this.clientId = clientId;
            this.status = OfferStatus.UTWORZONA;
       
            Closed = false;
        }

        public void setProject(bool isConstruction = false)
        {
            string pname = "P" + this.offerId.Substring(1);
            if (isConstruction)
                this.construction = pname;
            else
                this.production = pname;
        
        }

        public void setProjectAndCopy(bool isConstruction = false)
        {
            setProject(isConstruction);
            FileHandler.setupProjectFolder(this);
        }


        public void loadPhoto(string path)
        {
            if(this.photoPath != null)
                ImageCache.Unload(this.photoPath);
            this.photoPath = path;
            this.Photo = ImageCache.SafeGet(this.photoPath);
            
        }

        public override string ToString()
        {
            return String.Join(" ", [this.orderNumber, this.orderPath, this.offerId]);
        }

        

        public static Offer TemplateConstructor()
        {
            return new Offer
            {
                offerId = "N00" + StringHandler.RandomString(4),
                Photo = ImageCache.LOGO,
                description = StringHandler.RandomString(20),
                quantity = 0,
                name = StringHandler.RandomString(7),
                clientId = 0,
                Closed = false
            } ; 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
    }
}
