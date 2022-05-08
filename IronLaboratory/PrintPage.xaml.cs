using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for PrintPage.xaml
    /// </summary>
    public partial class PrintPage : Page
    {
        int _count;
        Frame _frame;
        SampleViewModel _sampleViewModel;
        List<int> _list;
        PrintQueue _mevaPrinter;

        public PrintPage()
        {
            InitializeComponent();
        }

        //Constructor for multiple records
        public PrintPage(Frame f, SampleViewModel svm, int c, List<int> l)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;
            _count = c;
            _list = l;
            Main_Loaded();
        }

        //Constructor for one record
        public PrintPage(Frame f, SampleViewModel svm, List<int> l, Int64 pId)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;
            _list = l;
            OneSample_Loaded(pId);
        }

        private void Main_Loaded()
        {
            _mevaPrinter = GetPrinter();
            Int64 _lastId = ReturnTopId();
            CreateLabels fixedDocument = new CreateLabels();
            _DocumentViewer.Document = fixedDocument.CreateDocument(GetSamples(_count, _lastId), _list);
        }

        private void OneSample_Loaded(Int64 primaryId)
        {
            _mevaPrinter = GetPrinter();
            CreateLabels fixedDocument = new CreateLabels();
            _DocumentViewer.Document = fixedDocument.CreateDocument(GetOneSample(primaryId), _list);
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PrintButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private DataTable GetSamples(int numberOfrows, Int64 lastId)
        {
            DataTable dataTable = new DataTable();

            if (lastId > 0)
            {
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
                {
                    SqlCommand select = new SqlCommand("recordsReturnAll", conn);
                    conn.Open();
                    select.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter("@pPrimaryId", SqlDbType.BigInt);

                    Int64 pId = lastId - numberOfrows + 1;

                    param.Value = pId;
                    select.Parameters.Add(param);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(select);
                    sqlDataAdapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetOneSample(Int64 pId)
        {
            DataTable dataTable = new DataTable();

            if (pId > 0)
            {
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
                {
                    SqlCommand sc = new SqlCommand("recordReturnByPrimaryId", conn);
                    conn.Open();
                    sc.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter("@pPrimaryId", SqlDbType.BigInt)
                    {
                        Value = pId
                    };
                    sc.Parameters.Add(param);

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sc);
                    sqlDataAdapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private Int64 ReturnTopId()
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }

                SqlCommand sc = new SqlCommand("returnLastPrimaryId", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;
                Int64 newId = (Int64)sc.ExecuteScalar();

                return newId;
            }
        }

        private PrintQueue GetPrinter()
        {
            var printers = new LocalPrintServer().GetPrintQueues();
            var selectedPrinter = printers.FirstOrDefault(p => p.Name == "MEVA MBP-1000");
            return selectedPrinter;
        }

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_mevaPrinter == null)
            {
                MessageBox.Show("Printer not found, make sure the printer software is installed on your computer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            var writer = PrintQueue.CreateXpsDocumentWriter(_mevaPrinter);
            var paginator = _DocumentViewer.Document.DocumentPaginator;
            writer.Write(paginator);

            Thread.Sleep(3000);
            _mevaPrinter.Refresh();
            if (_mevaPrinter.NumberOfJobs != 0)
            {
                MessageBox.Show("If labels are not printed yet, make sure:" + "\n" + "* The printer software is installed." + "\n" + "* The printer is connected to your computer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mevaPrinter == null)
            {
                MessageBox.Show("Printer not found, make sure the printer software is installed on your computer.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            var writer = PrintQueue.CreateXpsDocumentWriter(_mevaPrinter);
            var paginator = _DocumentViewer.Document.DocumentPaginator;
            writer.Write(paginator);

            Thread.Sleep(3000);
            _mevaPrinter.Refresh();
            if (_mevaPrinter.NumberOfJobs != 0)
            {
                MessageBox.Show("If labels are not printed yet, make sure:" + "\n" + "* The printer software is installed." + "\n" + "* The printer is connected to your computer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }
    }
}
