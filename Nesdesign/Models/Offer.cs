using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Nesdesign.Models
{

    [Table("Offers")]

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
        [Column("quantity2")]
        private string? quantity2 = "1";

        [ObservableProperty]
        [Column("name")]
        private string name;

        [ObservableProperty]
        [Column("clientId")]
        private int? clientId = null;


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
        [Column("ContractorId")]
        private int? contractorId;

        [Column("who")]
        private string who = "";




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
        private decimal? price;

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
                UpdateOrder();

            }
        }






        [NotMapped]
        public ObservableCollection<Part> PartNames { get; set; } 
        
        public void UpdateOrder()
        {
            OnPropertyChanged(nameof(IsOrder));
            OnPropertyChanged(nameof(ShouldDiplayOrder));
            OnPropertyChanged(nameof(ShouldDisplayCreateOrder));
        }


        

        public string DaysLeftToDate1 => (Date1.HasValue) ? "(" + (Date1.Value - DateTime.Now).Days + " dni)" : "";
        public string DaysLeftToDate2 => (Date2.HasValue) ? "(" + (Date2.Value - DateTime.Now).Days + " dni)" : "";

        public string SortableOfferID => OfferId.Substring(5, 2) + OfferId.Substring(3, 2) + OfferId.Substring(1, 2);

        [NotMapped]
        public string AllInfo => $"{OfferId} {Description} {orderNumber} {name} {orderPath} {projectPath} ";

        [NotMapped]
        public bool IsOrder => (int)Status >= (int)OfferStatus.ZAMOWIENIE;
        public bool ShouldDisplayCreateOrder => string.IsNullOrEmpty(OrderNumber) && IsOrder;
        public bool ShouldDiplayOrder => !(string.IsNullOrEmpty(OrderNumber)) && IsOrder;

        public string statusString => StringHandler.GetEnumString(this.Status);
        

        [ObservableProperty]
        private bool closed = false;

        public string projectPath => !String.IsNullOrEmpty(Construction) ? Construction : Production;

        public Offer() {
        }

        public Offer(string OfferId, ImageSource imageSource, string description, int quantity, string name)
        {
            this.OfferId = OfferId;
            this.Photo = imageSource;
            this.description = description;
            this.quantity = quantity;
            this.name = name;
            this.clientId = null;
            this.Status = OfferStatus.UTWORZONA;

            Closed = false;
            LoadParts();


        }

        public void LoadParts() {
            PartNames = new ObservableCollection<Part>();
            
            string[] names = this.name.Split("\n");
            foreach (string pName in names)
            {
                string t = pName.Trim();
                if(!(t.EndsWith("?1") || t.EndsWith("?0")))
                {
                    t += "?1";
                }
                
                PartNames.Add(new Part(t.Split("?")[0], Int16.Parse(t.Split("?")[1]) > 0, this));
            }
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

        public void NewPartsText()
        {
            string t = "";
            foreach(Part p in PartNames)
            {
                t += p.Name.Trim() + "?" + (p.Checked ? "1" : "0") + "\n";
            }
            this.name = t.Trim();
            OnPropertyChanged(nameof(Name));
        }

        /*
       public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
          //  => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
*/

        
        public void AddPart()
        {
            this.PartNames.Add(new Part("", true, this));
            NewPartsText();
        }

        public void DeletePart(Part part)
        {
            
            if (part != null)
            {
                if(PartNames.Count > 1)
                {
                    PartNames.Remove(part);
                    NewPartsText();
                } else
                {
                    part.Name = "";
                }

            }
        }

        public void UpdateAllInfo() { 
        
        OnPropertyChanged(nameof(AllInfo));
        }

    }

}
