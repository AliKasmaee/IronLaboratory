using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for AddOthersPage.xaml
    /// </summary>
    public partial class AddOthersPage : Page
    {
        Frame _frame;
        SampleViewModel _sampleViewModel;
        DataTable _dataTable;
        ObservableCollection<TempSample> samples;

        public AddOthersPage()
        {
            InitializeComponent();
        }

        public AddOthersPage(Frame f, SampleViewModel svm)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;
            RadOnDate.Culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            samples = new ObservableCollection<TempSample>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime gregorianDate = DateTime.Today;
            RadOnDate.SelectedDate = gregorianDate;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.NavigationService.GoBack();
        }

        private void AddTempButton_Click(object sender, RoutedEventArgs e)
        {
            if ((MaterialTextBox.Text == "") || (ExperimentTextBox.Text == ""))
            {
                MessageBox.Show("ابتدا اطلاعات نمونه را وارد کنید", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
            else
            {
                TempSample tempSample = new TempSample()
                {
                    Registration = RegistrationTextBox.Text,
                    Material = MaterialTextBox.Text,
                    Experiment = ExperimentTextBox.Text,
                    Clock = ClockTextBox.Text,
                    OnDate = DateConversion.ConvertToPersian((DateTime)RadOnDate.SelectedDate),
                    OrderId = OrderIdTextBox.Text,
                    Number = (int)NumberRad.Value
                };

                samples.Add(tempSample);
                RadDataGrid.ItemsSource = samples;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (samples.Count == 0)
            {
                MessageBox.Show("ابتدا نمونه ها را اضافه کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
            else if (samples.Count > 0)
            {
                var allSamples = new List<TempSample>(RadDataGrid.ItemsSource as IEnumerable<TempSample>);
                _dataTable = ConvertToDataTable.ToDataTable(allSamples);
                int numberOfInserted = _sampleViewModel.AddDataTableToSampleRepository(_dataTable);

                List<int> numberList = new List<int>();

                foreach (TempSample item in allSamples)
                {
                    numberList.Add(item.Number);
                }

                var result = MessageBox.Show("تعداد نمونه های اضافه شده: " + numberOfInserted + "\n" + "------------------------------" + "\n" + "\n" + "نمونه ها چاپ شوند؟", "Report", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);

                if (result == MessageBoxResult.Yes)
                {
                    //_frame.NavigationService.Navigate(new PrintPage(allSamples.Count, _frame, _sampleViewModel, numberList));
                    PrintWindow printWindow = new PrintWindow(_sampleViewModel, allSamples.Count, numberList);
                    printWindow.Show();
                }
            }
        }

        private void RadDataGrid_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            samples.Remove((TempSample)RadDataGrid.SelectedItem);
        }
    }
}
