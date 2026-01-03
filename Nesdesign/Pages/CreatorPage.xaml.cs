using Nesdesign.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

    public partial class CreatorPage : Page
    {
        private readonly OffersViewModel _viewModel;

        public CreatorPage(OffersViewModel offersViewModel)
        {
            InitializeComponent();
            _viewModel = offersViewModel;
            DataContext = _viewModel;
            YearTextBox.Text = DateTime.Now.Year.ToString();
        }

        private void CreatorCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if(MonthYearPanel != null)
                if (CreatorCombobox.SelectedIndex == 2)
                    MonthYearPanel.Visibility = Visibility.Visible;
                else
                    MonthYearPanel.Visibility = Visibility.Collapsed;
        }

    

        private void CreateOfferClick(object sender, RoutedEventArgs e)
           
        {
            string projectName = CreateOfferId(2025,1);
            int selectedIndex = CreatorCombobox.SelectedIndex;
            switch(selectedIndex)
            {

                case 0:
                    if (_viewModel.LatestOfferId != "N0000000")
                    {
                        string t = _viewModel.LatestOfferId.Substring(3, 4);
                        projectName = CreateOfferId(t);
                    } else
                    {
                        DateTime now_ = DateTime.Now;
                        projectName = CreateOfferId(now_.Year, now_.Month);
                    }

                        break;

                case 1: 
                    DateTime now = DateTime.Now;
                    projectName = CreateOfferId(now.Year, now.Month);
                 break;

                case 2:
                    int year = Int16.Parse(this.YearTextBox.Text);
                    int month = SelectMonth.SelectedIndex +1;
                    projectName = CreateOfferId(year, month);
                    //int year = int.Parse(YearTextBox.Text);
                    //int month = int.Parse(MonthTextBox.Text);
                    projectName = CreateOfferId(year, month);
                    break;

            }
            
  
            FileOperationStatus status = FileHandler.CreateDir(projectName, DIR_TYPE.Offer);
            switch (status) {
                case FileOperationStatus.Success:
                    MessageLabel.Content = "Utworzono pomyślnie projekt: " + projectName;
                    break;
                case FileOperationStatus.AlreadyExists:
                    MessageLabel.Content = "Oferta o nazwie " + projectName + " już istnieje.";
                    break;
                case FileOperationStatus.AccessDenied:
                    MessageLabel.Content = "Odmowa dostępu";
                    break;

            }

            _viewModel.Offers.Add(new Offer(projectName, ImageHandler.LOGO,"",1,"",1));
            

        }

        private void CreateProjectClick(object sender, RoutedEventArgs e)
        {
            int ind = ProjectTypeComboBox.SelectedIndex;
            Offer selectedOffer = _viewModel.SelectedItem as Offer;
            selectedOffer.setProject(Convert.ToBoolean(ind));
            string projectName = selectedOffer.projectPath;
            FileOperationStatus status = FileHandler.CreateDir(projectName, DIR_TYPE.Project);
            switch (status)
            {
                case FileOperationStatus.Success:
                    MessageLabel2.Content = "Utworzono pomyślnie projekt: " + projectName;
                    break;
                case FileOperationStatus.AlreadyExists:
                    MessageLabel2.Content = "Projekt o nazwie " + projectName + " już istnieje.";
                    break;
                case FileOperationStatus.AccessDenied:
                    MessageLabel2.Content = "Odmowa dostępu";
                    break;

            }


        }

        private int getCount(string suffix)
        {
            string pattern = $"^N\\d{{2}}{suffix}$";
            Regex regex = new Regex(pattern);
            return _viewModel.CountOffersMatching(pattern);
        }
        private int getCount(int year, int month)
        {
            string mm = month.ToString("00");
            string yy = (year % 100).ToString("00"); // two-digit year

           return getCount(mm + yy);
        }


        public string CreateOfferId(string suffix)
        {
            string n = "N";
            string nr = string.Format("{0:00}", getCount(suffix) + 1);

            n += nr + suffix;
            return n;
        }
        public string CreateOfferId(int year, int month)
        {
            string n = "N";
            string month_ = string.Format("{0:00}", month);
            string year_ = year.ToString();
            year_ = year_.Substring(year_.Length - 2);
            string suffix = month_ + year_;

            return CreateOfferId(suffix);
        }
           





        private void OpenOfferFolderClick(object sender, RoutedEventArgs e)
        {
            FileHandler.OpenFolder(_viewModel.SelectedOfferId);
        }

        private void OpenProjectFolderClick(object sender, RoutedEventArgs e)
        {
            FileHandler.OpenFolder(_viewModel.SelectedItem.projectPath, DIR_TYPE.Project);
        }

        private void CopyStructureClick(object sender, RoutedEventArgs e)
        {
            Offer offer = _viewModel.SelectedItem as Offer;
            Button btn = sender as Button;
            string tag = btn.Tag.ToString();
            string dirname;
            DIR_TYPE dir_type;
            if(tag == "offer")
            {
                dirname = offer.OfferId;
                dir_type = DIR_TYPE.Offer;
            }
            else
            {
                dirname = offer.projectPath;
                dir_type = DIR_TYPE.Project;
            }
       
   
            

            FileOperationStatus status = FileHandler.CopyTemplate(dirname, dir_type);
            switch (status)
            {
                case FileOperationStatus.Success:
                    MessageLabel.Content = "Kopiowanie powiodło się!";
                    break;
                default:
                    MessageLabel.Content = "Wystąpił błąd";
                    break;
            }
            }



        private void Filter(OfferStatus status)
        {
            this._viewModel.FilterByStatus(status);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

        private void CreatedFilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.UTWORZONA);
        }
        private void Option2FilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.OFERTA);
        }

        private void Option3FilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.ZAMOWIENIE);
        }

        private void InProgressFilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.W_REALIZACJI);
        }

        private void ReadyFilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.GOTOWA);
        }
        private void FinishedFilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.ZAKONCZONA);
        }

        private void ClearFilterClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilter();
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }



    }
}
