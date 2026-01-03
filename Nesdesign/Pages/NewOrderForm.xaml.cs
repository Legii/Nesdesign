using Nesdesign.Models;
using System;
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
    /// Logika interakcji dla klasy NewOrder.xaml
    /// </summary>
    /// 


    public enum FormResult {SUBMITTED, CANCELLED, ERROR}

    public partial class NewOrder : UserControl
    {
        private Action<Offer>? _callback;


        public NewOrder()
        {
            InitializeComponent();
        }

        public void SetCallback(Action<Offer> callback)
        {
            _callback = callback;
        }

        private Offer? _editingOffer;

        public void LoadOrder(Offer offer)
        {
            _editingOffer = offer;

            txtOrderNumber.Text = offer.OrderNumber;
            TxtOrderDirPath.Text = offer.OrderPath;
        }



        private void Submit_Click(object sender, RoutedEventArgs e)
        {

            _editingOffer.OrderNumber = txtOrderNumber.Text;
            _editingOffer.OrderPath = OrderMode.SelectedIndex == 0? txtOrderNumber.Text :TxtOrderDirPath.Text;

            _callback?.Invoke(_editingOffer);

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _callback?.Invoke(null);
        }



 

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int ind = OrderMode.SelectedIndex;
            if(LabelOrderDirPath != null && TxtOrderDirPath != null)
                if (ind == 0)
                {
                    LabelOrderDirPath.Visibility = Visibility.Collapsed;
                    TxtOrderDirPath.Visibility = Visibility.Collapsed;
                }
                else
                {
                    LabelOrderDirPath.Visibility = Visibility.Visible;
                    TxtOrderDirPath.Visibility = Visibility.Visible;
                }
        }
    }
}
