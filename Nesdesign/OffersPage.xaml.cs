using Nesdesign.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nesdesign
{
    /// <summary>
    /// Logika interakcji dla klasy Zapytania.xaml
    /// </summary>
    public partial class OffersPage : Page
    {
      
        public OffersPage(OffersViewModel offersViewModel)
        {
            InitializeComponent();
            
            this.DataContext = offersViewModel;
            for (int i = 0; i < 100000; i++)
               offersViewModel.Offers.Add(Offer.TemplateConstructor());
            //OffersDataGrid.Items.Add(Offer.TemplateConstructor());
        }
    }
}
