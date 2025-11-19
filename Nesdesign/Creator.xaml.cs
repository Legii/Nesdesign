using Nesdesign.Models;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Logika interakcji dla klasy Creator.xaml
    /// </summary>
    public partial class Creator : Page
    {
        private readonly OffersViewModel _viewModel;

        public Creator(OffersViewModel offersViewModel)
        {
            InitializeComponent();
            _viewModel = offersViewModel;
            DataContext = _viewModel;
        }

        private void CreateOfferClick(object sender, RoutedEventArgs e)
        {
            string projectName = CreateOfferId(2025,8);
            FileOperationStatus status = FileHandler.CreateDir(projectName);
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
            _viewModel.Offers.Add(new Offer(projectName, "","",1,"",0));

        }

        private void CreateProjectClick(object sender, RoutedEventArgs e)
        {
            string projectName = _viewModel.Selected;
            projectName = "P" + projectName.Substring(1);
            FileOperationStatus status = FileHandler.CreateDir(projectName, true);
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

            _viewModel.SelectedProject = projectName;

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
            string nr = CreateOfferId(suffix).ToString();

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

            string nr = (getCount(suffix) + 1).ToString("00");

            n += nr + month_ + year_;
            return n;
        }
           





        private void OpenOfferFolderClick(object sender, RoutedEventArgs e)
        {
            FileHandler.OpenFolder(_viewModel.LatestOfferId);
        }

        private void OpenProjectFolderClick(object sender, RoutedEventArgs e)
        {
            FileHandler.OpenFolder(_viewModel.SelectedProject, true);
        }

        private void CopyStructureClick(object sender, RoutedEventArgs e)
        {
            FileOperationStatus status = FileHandler.CopyTemplate(_viewModel.LatestOfferId + "//");
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
