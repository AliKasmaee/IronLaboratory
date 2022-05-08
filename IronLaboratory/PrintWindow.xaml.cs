using System;
using System.Collections.Generic;
using System.Windows;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintWindow()
        {
            InitializeComponent();
        }

        //Constructor for multiple records
        public PrintWindow(SampleViewModel svm, int c, List<int> l)
        {
            InitializeComponent();
            PrintFrame.Navigate(new PrintPage(PrintFrame, svm, c, l));
        }

        //Constructor for one record
        public PrintWindow(SampleViewModel svm, List<int> l, Int64 pId)
        {
            InitializeComponent();
            PrintFrame.Navigate(new PrintPage(PrintFrame, svm, l, pId));
        }
    }
}
