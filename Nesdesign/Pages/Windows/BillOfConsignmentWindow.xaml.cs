using Nesdesign.Handlers;
using Nesdesign.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nesdesign.Pages.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy BillOfConsignmentWindow.xaml
    /// </summary>
    public partial class BillOfConsignmentWindow : Window
    {
        public BillData BillData_ { get; set; }
        /*
        public  Offer Offer_ { get; set; }
        private Client client_ { get; set; }
        public string ClientName { get; set; } = "";
        public string ClientAddress { get; set; } = "";
        public string BillNr { get; set; }
        public string OrderNumber { get; set; } = "";*/

        public BillOfConsignmentWindow()
        {
            InitializeComponent();

        }
        public int? ToInt(string q)
        {
            try
            {
                return Int16.Parse(q.Trim());
            } catch { }
            return null;
           
        }


        public BillOfConsignmentWindow(Offer offer,Client client_) : this()
        {
           

            string BillNr = (offer.projectPath != "" ? offer.projectPath : "PXYMMRR") + "_" + DateTime.Today.ToString("yyyyMMdd");
           
            List<Part> parts = offer.PartNames.ToList();
            Console.WriteLine(parts.Count);
            int i = 0;
            string[] quantities = offer.Quantity2.Trim().Split('\n');
            foreach (Part part in parts)
            {
                if (i < quantities.Length)
                {
                    
                    part.Quantity = ToInt(quantities[i]);
                    
                }
              
                i++;
            }
       
            int n = quantities.Length - i;
            Console.WriteLine("n" + n);
            
            for (int j = 0; j<n; j++)
            {
                parts.Add(new Part { Name="", Quantity = ToInt(quantities[i+j]), Checked=true });
            }
            this.BillData_ = new BillData(client_.Name, client_.Address, client_.Description, BillNr, offer.OrderNumber, parts);
            this.DataContext = this.BillData_;
        }


        private void GeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            
           // PdfHandler.CreatePDF();
            //MessageBox.Show("Generowanie PDF-a...");

        }
    }
}
