using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        Frame _frame;
        SampleViewModel _sampleViewModel;
        private Sample _sample;

        public SearchPage()
        {
            InitializeComponent();
        }

        public SearchPage(Frame f, SampleViewModel svm)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;

            RadOnDate.Culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            RadDateBox.Culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";

            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            PrintButton.IsEnabled = false;
            MyGrid.Visibility = Visibility.Hidden;
        }

        private void GridTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridTable.SelectedCells.Count == 0)
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                PrintButton.IsEnabled = false;
                return;
            }

            EditButton.IsEnabled = true;
            DeleteButton.IsEnabled = true;
            PrintButton.IsEnabled = true;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.Visibility = Visibility.Hidden;

            if (SampleIdBox.Text == "" && RegistrationBox.Text == "" && MaterialBox.Text == "" && !RadDateBox.SelectedDate.HasValue)
            {
                MessageBox.Show("تمام مقادیر خالی هستند", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                return;
            }

            GridTable.DataContext = null;
            GridTable.DataContext = GetSamplesByDetails();

            if (GridTable.SelectedCells.Count == 0)
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                PrintButton.IsEnabled = false;
            }
            else
            {
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
                PrintButton.IsEnabled = true;
            }

            if (GridTable.Items.Count == 0)
            {
                MessageBox.Show("نمونه ای با این مشخصات پیدا نشد", "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _sample = (Sample)GridTable.SelectedItem;
            SampleIdTextBox.Text = _sample.SampleId.ToString();
            RegistrationTextBox.Text = _sample.Registration;
            MaterialTextBox.Text = _sample.Material;
            ExperimentTextBox.Text = _sample.Experiment;
            ClockTextBox.Text = _sample.Clock;
            RadOnDate.SelectedDate = Convert.ToDateTime(DateConversion.ConvertToGregorian(_sample.OnDate));
            OrderIdTextBox.Text = _sample.OrderId;

            MyGrid.Visibility = Visibility.Visible;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Sample tempSample = (Sample)GridTable.SelectedItem;
            Int64 pId = tempSample.PrimaryId;

            var warning = MessageBox.Show("نمونه حذف شود؟", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.RightAlign);
            if (warning == MessageBoxResult.Yes)
            {
                _sampleViewModel.DeleteRecordFromRepository(pId);
                MessageBox.Show("نمونه با موفقیت حذف شد", "Delete", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }

            GridTable.DataContext = GetSamplesByDetails();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            List<Sample> selectedList = new List<Sample>();
            Sample s = (Sample)GridTable.SelectedItem;
            selectedList.Add(s);

            List<int> numberList = new List<int>();
            int x = 1;
            numberList.Add(x);

            //_frame.NavigationService.Navigate(new PrintPage(_frame, _sampleViewModel, numberList, s.PrimaryId));
            PrintWindow printWindow = new PrintWindow(_sampleViewModel, numberList, s.PrimaryId);
            printWindow.Show();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Sample tempSample = new Sample();
            if (!(_sample.Registration.Equals(RegistrationTextBox.Text)
                && _sample.Material.Equals(MaterialTextBox.Text)
                && _sample.Experiment.Equals(ExperimentTextBox.Text)
                && _sample.Clock.Equals(ClockTextBox.Text)
                && _sample.OnDate.Equals(DateConversion.ConvertToPersian((DateTime)RadOnDate.SelectedDate))
                && _sample.OrderId.Equals(OrderIdTextBox.Text)))
            {
                //uneditable
                tempSample.PrimaryId = _sample.PrimaryId;
                tempSample.SampleId = _sample.SampleId;
                tempSample.DateAndTimeAdded = _sample.DateAndTimeAdded;

                //editable
                tempSample.Registration = RegistrationTextBox.Text;
                tempSample.Material = MaterialTextBox.Text;
                tempSample.Experiment = ExperimentTextBox.Text;
                tempSample.Clock = ClockTextBox.Text;
                tempSample.OnDate = RadOnDate.SelectedDate.ToString();
                tempSample.OrderId = OrderIdTextBox.Text;

                _sampleViewModel.UpdateRecordInRepository(tempSample);
                MessageBox.Show("نمونه با موفقیت ویرایش شد", "Update", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);

                GridTable.DataContext = GetSamplesByDetails();
            }
            else
            {
                MessageBox.Show("ابتدا اطلاعات نمونه را تغییر دهید", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.NavigationService.GoBack();
        }

        private List<Sample> GetSamplesByDetails()
        {
            List<Sample> listOfSamples = new List<Sample>();

            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }

                SqlCommand sc = new SqlCommand("recordsReturnByDetails", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrWhiteSpace(SampleIdBox.Text))
                {
                    sc.Parameters.Add("@pSampleId", SqlDbType.Int).Value = Convert.ToInt32(SampleIdBox.Text);
                }

                if (!string.IsNullOrWhiteSpace(RegistrationBox.Text))
                {
                    sc.Parameters.Add("@pRegistration", SqlDbType.NVarChar).Value = RegistrationBox.Text.ToString();
                }

                if (!string.IsNullOrWhiteSpace(MaterialBox.Text))
                {
                    sc.Parameters.Add("@pMaterial", SqlDbType.NVarChar).Value = MaterialBox.Text.ToString();
                }

                if (RadDateBox.SelectedDate.HasValue)
                {
                    sc.Parameters.Add("@pOnDate", SqlDbType.Date).Value = RadDateBox.SelectedDate;
                }

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sc);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    Sample s = new Sample()
                    {
                        PrimaryId = (Int64)row["PrimaryId"],
                        SampleId = (int)row["SampleID"],
                        Registration = row["Registration"].ToString(),
                        Material = row["Material"].ToString(),
                        Experiment = row["Experiment"].ToString(),
                        Clock = row["Clock"].ToString(),
                        OnDate = DateConversion.ConvertToPersian((DateTime)row["OnDate"]),
                        OrderId = row["OrderID"].ToString(),
                        DateAndTimeAdded = DateConversion.TimeConversion(((DateTime)row["DateAndTimeAdded"]).ToShortTimeString()) + "    " + DateConversion.ConvertToPersian(Convert.ToDateTime(((DateTime)row["DateAndTimeAdded"]).ToShortDateString()))
                    };

                    listOfSamples.Add(s);
                }

                //listOfSamples.Sort((x, y) => y.PrimaryId.CompareTo(x.PrimaryId));
                return listOfSamples;
            }
        }
    }
}
