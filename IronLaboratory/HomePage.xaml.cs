using System.Windows;
using System.Windows.Controls;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Frame _frame;
        SampleViewModel _sampleViewModel;
        DataViewModel _dataViewModel;

        public HomePage()
        {
            InitializeComponent();
        }

        public HomePage(Frame f, SampleViewModel svm, DataViewModel dvm)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;
            _dataViewModel = dvm;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new AddPage1(_frame, _sampleViewModel, _dataViewModel));
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new AddPage2(_frame, _sampleViewModel, _dataViewModel));
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new AddPage3(_frame, _sampleViewModel, _dataViewModel));
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new AddPage4(_frame, _sampleViewModel, _dataViewModel));
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new AddNewPage(_frame, _sampleViewModel, _dataViewModel));
        }

        private void Others_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new AddOthersPage(_frame, _sampleViewModel));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SampleViewModel view = new SampleViewModel();
            _frame.Navigate(new SearchPage(_frame, view));
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            SampleViewModel view = new SampleViewModel();
            _frame.Navigate(new ReportPage(_frame, view));
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
