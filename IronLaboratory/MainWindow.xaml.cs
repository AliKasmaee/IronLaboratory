using System.Windows;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SampleViewModel ViewModel { get; set; }
        public DataViewModel DataViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new SampleViewModel();
            DataViewModel = new DataViewModel();
            Frame.Navigate(new HomePage(Frame, ViewModel, DataViewModel));
        }
    }
}
