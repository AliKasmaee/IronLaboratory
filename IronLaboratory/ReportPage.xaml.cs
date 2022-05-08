using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        Frame _frame;
        SampleViewModel _sampleViewModel;
        DataTable _dataTable;

        public Microsoft.Office.Interop.Excel.Application APP = null;
        public Microsoft.Office.Interop.Excel.Workbook WB = null;
        public Microsoft.Office.Interop.Excel.Worksheet WS = null;
        public Microsoft.Office.Interop.Excel.Range Range = null;

        public ReportPage()
        {
            InitializeComponent();
        }

        public ReportPage(Frame f, SampleViewModel svm)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;
            RadDateBox.Culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GridTable.DataContext = GetTopSamples();
        }

        private List<Sample> GetTopSamples()
        {
            List<Sample> listOfSamples = new List<Sample>();

            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }

                SqlCommand sc = new SqlCommand("returnTop10Rows", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;

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
                return listOfSamples;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SampleIdBox.Text == "" && RegistrationBox.Text == "" && MaterialBox.Text == "" && !RadDateBox.SelectedDate.HasValue)
            {
                MessageBox.Show("تمام مقادیر خالی هستند", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                return;
            }

            GridTable.DataContext = null;
            GridTable.DataContext = GetSamplesByDetails();

            if (GridTable.Items.Count == 0)
            {
                MessageBox.Show("نمونه ای با این مشخصات پیدا نشد", "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
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
                return listOfSamples;
            }
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            string today = DateConversion.ConvertToPersian(DateTime.Today) + "--" + DateConversion.TimeWithSecondsConversion(DateTime.Now.ToLongTimeString());
            today = today.Replace('/', '-');
            today = today.Replace(':', '-');

            var allSamples = new List<Sample>(GridTable.ItemsSource as IEnumerable<Sample>);
            _dataTable = ConvertToDataTable.ToDataTable(allSamples);

            APP = new Microsoft.Office.Interop.Excel.Application();
            WB = APP.Workbooks.Add(1);
            WS = (Microsoft.Office.Interop.Excel.Worksheet)WB.Sheets[1];
            WS.DisplayRightToLeft = true;
            WS.Cells.Font.Size = 14;

            int ind = 1;
            int rowcount = 1;

            //Headers
            foreach (object ob in this.GridTable.Columns.Select(cs => cs.Header).ToList())
            {
                WS.Cells[1, ind] = ob.ToString();
                ind++;
            }

            //Rows
            foreach (DataRow row in _dataTable.Rows)
            {
                rowcount += 1;

                //Columns
                for (int i = 0; i < this.GridTable.Columns.Count; i++)
                {
                    WS.Cells[rowcount, i + 1] = row[i].ToString();
                }
            }

            Microsoft.Office.Interop.Excel.Range cellRange = WS.Range[WS.Cells[1, 1], WS.Cells[rowcount, this.GridTable.Columns.Count]];
            cellRange.EntireColumn.AutoFit();
            cellRange.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            Microsoft.Office.Interop.Excel.Range range = WS.Range[WS.Cells[1, 1], WS.Cells[1, this.GridTable.Columns.Count]];
            range.Font.Bold = true;

            SaveFileDialog saveFile = new SaveFileDialog
            {
                FileName = today,
                Filter = "Excel Files|*.xlsx",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "Export Excel File To"
            };
            Nullable<bool> result = saveFile.ShowDialog();

            if (result == true)
            {
                string path = saveFile.FileName;
                WB.SaveCopyAs(path);
                WB.Saved = true;
                WB.Close(true, Type.Missing, Type.Missing);
                APP.Quit();
            }
        }
    }
}
