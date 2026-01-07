using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Nesdesign.Models
{


    public partial class Offer : ObservableObject
    {
        [ObservableProperty]
        [Column("offerId")]
        private string offerId;




        [Column("photoPath")]
        [ObservableProperty]
        private string photoPath = "";
      
      

        [NotMapped]
        public ImageSource Photo { get; set; }

        [ObservableProperty]
        [Column("description")]
        private string description = "";

        [ObservableProperty]
        [Column("quantity")]
        private int quantity = 1;

        [ObservableProperty]
        [Column("name")]
        private string name;

        [ObservableProperty]
        [Column("clientId")]
        private int? clientId;


        [ObservableProperty]
        [Column("orderNumber")]
        private string? orderNumber = "";

        [ObservableProperty]
        [Column("orderPath")]
        private string? orderPath = "";

        [ObservableProperty]
        [Column("construction")]
        private string? construction = "";

        [ObservableProperty]
        [Column("production")]
        private string? production = "";

        [ObservableProperty]
        [Column("who")]
        private string? who = "";

        [ObservableProperty]
        [Column("date1")]
        private DateTime? date1;

        [ObservableProperty]
        [Column("date2")]
        private DateTime? date2;

        [ObservableProperty]
        [Column("invoiceNumber")]
        private string? invoiceNumber = "";

        [ObservableProperty]
        [Column("price")]
        private int? price =100;

        [ObservableProperty]
        [Column("shipment")]
        private bool shipment = false;

        [Column("paymentStatus")]
        [ObservableProperty]
        private PaymentStatus _paymentStatus = Models.PaymentStatus.NIEWYSTAWIONA;


        [NotMapped]
        private OfferStatus _status;
        
        public OfferStatus Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(statusString));
            }
        }
        
        public string DaysLeftToDate1 => (Date1.HasValue) ? "(" + (Date1.Value - DateTime.Now).Days + " dni)" : "";
        public string DaysLeftToDate2 => (Date2.HasValue) ? "(" + (Date2.Value - DateTime.Now).Days + " dni)" : "";

        [NotMapped]
        public string AllInfo => $"{OfferId} {Description} {orderNumber} {name} {orderPath} {projectPath} ";

        public string statusString => StringHandler.GetEnumString(this.Status);

        [ObservableProperty]
        private bool closed = false;

        public string projectPath => !String.IsNullOrEmpty(Construction) ? Construction : Production;

        public Offer() { }

        public Offer(string OfferId, ImageSource imageSource, string description, int quantity, string name, int clientId)
        {
            this.OfferId = OfferId;
            this.Photo = imageSource;
            this.description = description;
            this.quantity = quantity;
            this.name = name;
            this.clientId = clientId;
            this.Status = OfferStatus.UTWORZONA;

            Closed = false;
        }

        public void setProject(bool isConstruction = false)
        {
            string pname = "P" + this.OfferId.Substring(1);
            if (isConstruction)
                this.Construction = pname;
            else
                this.Production = pname;

        }

        public void setProjectAndCopy(bool isConstruction = false)
        {
            setProject(isConstruction);
            FileHandler.setupProjectFolder(this);
        }


        public void LoadPhoto(string path)
        {
            this.photoPath = path;
            if (this.photoPath != null)
                ImageHandler.Unload(this.photoPath);
            
            this.Photo = ImageHandler.SafeGet(this.photoPath);
            this.OnPropertyChanged(nameof(Photo));
        }

        public override string ToString()
        {
            string t = $"OfferId: {OfferId}, Name: {name}, Description: {description}, Quantity: {quantity}, ClientId: {clientId}, Status: {Status}";
            return t;
        }



        public static Offer TemplateConstructor()
        {
            return new Offer
            {
                offerId = "N00" + StringHandler.RandomString(4),
                Photo = ImageHandler.LOGO,
                description = StringHandler.RandomString(20),
                quantity = 0,
                name = StringHandler.RandomString(7),
                clientId = 0,
                Closed = false
            };
        }


       // public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
          //  => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void UpdateAllInfo() { 
        
        OnPropertyChanged(nameof(AllInfo));
        }

    }

}
