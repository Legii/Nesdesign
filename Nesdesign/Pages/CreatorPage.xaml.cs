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

            if (MonthYearPanel != null)
                if (CreatorCombobox.SelectedIndex == 2)
                    MonthYearPanel.Visibility = Visibility.Visible;
                else
                    MonthYearPanel.Visibility = Visibility.Collapsed;
        }


        public string CreateOfferNameFromVariant(int variant)
        {
            string offerName = "";
            switch (variant)
            {

                case 0:
                    if (_viewModel.LatestOfferId != "N0000000")
                    {
                        string t = _viewModel.LatestOfferId.Substring(3, 4);
                        offerName = CreateOfferId(t);
                    }
                    else
                    {
                        DateTime now_ = DateTime.Now;
                        offerName = CreateOfferId(now_.Year, now_.Month);
                    }

                    break;

                case 1:
                    DateTime now = DateTime.Now;
                    offerName = CreateOfferId(now.Year, now.Month);
                    break;

                case 2:
                    int year = Int16.Parse(this.YearTextBox.Text);
                    int month = SelectMonth.SelectedIndex + 1;
                    offerName = CreateOfferId(year, month);
                    break;

            }
            return offerName;
        }


        private void CreateOfferClick(object sender, RoutedEventArgs e)

        {
            string offerName = "";
            int selectedIndex = CreatorCombobox.SelectedIndex;
            offerName = CreateOfferNameFromVariant(selectedIndex);


            FileOperationStatus status = FileHandler.CreateDir(offerName, DIR_TYPE.Offer);
            switch (status)
            {
                case FileOperationStatus.Success:
                    MessageLabel.Content = "Utworzono pomyślnie projekt: " + offerName;
                    break;
                case FileOperationStatus.AlreadyExists:
                    MessageLabel.Content = "Oferta o nazwie " + offerName + " już istnieje.";
                    var result = MessageBox.Show($"Folder oferty o numerze {offerName} już istnieje, czy dodać do bazy?", "Potwierdź operację", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.Cancel)
                        return;

                    break;
                case FileOperationStatus.AccessDenied:
                    MessageLabel.Content = "Odmowa dostępu";
                    MessageBox.Show("Brak uprawnień");
                    return;
                default:
                    MessageBox.Show("Błąd");
                    return;


            }


            _viewModel.Offers.Add(new Offer(offerName, ImageHandler.LOGO, "", 1, "", 1));


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

        public void CreateOfferAndCopy()
        {
            string offerName = CreateOfferNameFromVariant(0);
            FileOperationStatus status = FileHandler.CreateDir(offerName, DIR_TYPE.Offer);
            bool Continue = false;
            if(status == FileOperationStatus.AlreadyExists)
            {
                var result = MessageBox.Show($"Folder oferty o numerze {offerName} już istnieje, czy dodać do bazy?", "Potwierdź operację", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
                else Continue = true;
            }

            if (status == FileOperationStatus.Success || Continue)
            {
                try
                {
                    Offer offer = new Offer(offerName, ImageHandler.LOGO, "", 1, "", 1);
                    _viewModel.Offers.Add(offer);
                    CopyOfferStructure(offer);
                }
                catch
                {
                    MessageBox.Show("Błąd podczas tworzenia oferty i kopiowania struktury");
                }
                MessageBox.Show("Utworzono ofertę: " + offerName);

            }
            
            else
            {
                MessageBox.Show("Nie udało się utworzyć oferty: " + offerName);
            }
        }

        private int getCount(string suffix)
        {
            string pattern = $"^N\\d{{2}}{suffix}$";
            Regex regex = new Regex(pattern);
            return _viewModel.CountOffersMatching(pattern);
        }


        public string CreateOfferId(string suffix)
        {
            var matching = _viewModel.Offers
                .Where(o => o.OfferId.EndsWith(suffix))
                .Select(o => o.OfferId);

            int lastNumber = 0;

            foreach (var id in matching)
            {

                string numberPart = id.Substring(1, id.Length - suffix.Length - 1);

                if (int.TryParse(numberPart, out int num))
                {
                    if (num > lastNumber)
                        lastNumber = num;
                }
            }

            int newNumber = lastNumber + 1;

            return $"N{newNumber:00}{suffix}";

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

        private void CopyOfferStructure(Offer offer)
        {
            FileOperationStatus status = FileHandler.CopyTemplate(offer.OfferId, DIR_TYPE.Offer);
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

        private void CopyStructureClick(object sender, RoutedEventArgs e)
        {
            Offer offer = _viewModel.SelectedItem as Offer;
            Button btn = sender as Button;
            string tag = btn.Tag.ToString();
            string dirname;
            Label label;

            DIR_TYPE dir_type;
            if (tag == "offer")
            {
                dirname = offer.OfferId;
                dir_type = DIR_TYPE.Offer;
                label = MessageLabel;
            }
            else
            {
                dirname = offer.projectPath;
                dir_type = DIR_TYPE.Project;
                label = MessageLabel2;

            }


            FileOperationStatus status = FileHandler.CopyTemplate(dirname, dir_type);
            switch (status)
            {
                case FileOperationStatus.Success:
                    label.Content = "Kopiowanie powiodło się!";
                    break;
                default:
                    label.Content = "Wystąpił błąd";
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
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }
        private void Option2FilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.OFERTA);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

        private void Option3FilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.NIE_OFERTOWANA);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

        private void InProgressFilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.W_REALIZACJI);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

        private void ReadyFilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.GOTOWA);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }

        }
        private void FinishedFilterClick(object sender, RoutedEventArgs e)
        {
            Filter(OfferStatus.ZAKONCZONA);
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

        private void ClearFilterClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilters();
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToOffersPage();
            }
        }

    }
}
