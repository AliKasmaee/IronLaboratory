using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for AddNewPage.xaml
    /// </summary>
    public partial class AddNewPage : Page
    {
        Frame _frame;
        SampleViewModel _sampleViewModel;
        DataViewModel _dataViewModel;
        ObservableCollection<TempData> datas;
        bool _anythingAdded;

        public AddNewPage()
        {
            InitializeComponent();
        }

        public AddNewPage(Frame f, SampleViewModel svm, DataViewModel dvm)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;
            _dataViewModel = dvm;

            RadTime.Culture.DateTimeFormat.ShortTimePattern = "H:mm";
            datas = new ObservableCollection<TempData>();
            _anythingAdded = false;
            MaterialTextBox.Focus();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //If new data is added, the DataViewModel is updated
            if (_anythingAdded)
            {
                _dataViewModel = new DataViewModel();
                _frame.NavigationService.Navigate(new HomePage(_frame, _sampleViewModel, _dataViewModel));
            }
            else
            {
                _frame.NavigationService.GoBack();
            }
        }

        private void ClockComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClockComboBox.SelectedIndex == 3)
            {
                OtherClocksTextBox.Visibility = Visibility.Visible;
                OtherClocksTextBox.Focus();
            }
            else
            {
                OtherClocksTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void AddTempButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrEmpty(MaterialTextBox.Text))
                || (string.IsNullOrEmpty(ExperimentTextBox.Text))
                || (string.IsNullOrEmpty(TypeOfComboBox.Text)))
            {
                MessageBox.Show("ابتدا اطلاعات نمونه را وارد کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                return;
            }
            else if ((!RadTime.IsEnabled) && (!ClockComboBox.IsEnabled))
            {
                MessageBox.Show("ساعت نمونه را انتخاب کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                return;
            }
            else if (RadTime.IsEnabled && RadTime.SelectedTime == null)
            {
                MessageBox.Show("ساعت نمونه را وارد کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                return;
            }
            else if (ClockComboBox.IsEnabled)
            {
                if (ClockComboBox.Text == null || ClockComboBox.SelectedItem == null)
                {
                    MessageBox.Show("ساعت نمونه را وارد کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }
                else if (ClockComboBox.SelectedIndex == 3 && string.IsNullOrWhiteSpace(OtherClocksTextBox.Text))
                {
                    MessageBox.Show("ساعت مورد نظر را در قسمت مربوطه وارد کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    return;
                }
            }

            TempData tempData = new TempData();
            tempData.Material = MaterialTextBox.Text;
            tempData.Experiment = ExperimentTextBox.Text;

            if (RadTime.IsEnabled)
            {
                string tcl = RadTime.SelectedTime.ToString();
                var value = tcl.Split(':');
                Int16 hour = Convert.ToInt16(value[0]);

                tempData.Clock = hour.ToString() + ":00";
                tempData.OnHour = hour;

                if (hour >= 5 && hour <= 19)
                {
                    tempData.OnShift = "شیفت روز";
                }
                else
                {
                    tempData.OnShift = "شیفت شب";
                }
            }
            else if (ClockComboBox.IsEnabled)
            {
                if (ClockComboBox.SelectedIndex == 3)
                {
                    tempData.Clock = OtherClocksTextBox.Text;
                }
                else
                {
                    tempData.Clock = ClockComboBox.Text;
                }

                tempData.OnHour = 1000;
                tempData.OnShift = "Empty";
            }

            tempData.Number = Convert.ToInt16(NumberRad.Value);

            if (TypeOfComboBox.SelectedIndex == 0)
            {
                tempData.TypeOf = 100;
                tempData.TypeOfSt = "پریودیک";
            }
            else if (TypeOfComboBox.SelectedIndex == 1)
            {
                tempData.TypeOf = 200;
                tempData.TypeOfSt = "ترن";
            }
            else if (TypeOfComboBox.SelectedIndex == 2)
            {
                tempData.TypeOf = 300;
                tempData.TypeOfSt = "بارنامه";
            }
            else if (TypeOfComboBox.SelectedIndex == 3)
            {
                tempData.TypeOf = 400;
                tempData.TypeOfSt = "هفتگی و ماهیانه";
            }

            datas.Add(tempData);
            RadDataGrid.ItemsSource = datas;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (datas.Count == 0)
            {
                MessageBox.Show("ابتدا نمونه ها را اضافه کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                return;
            }
            else
            {
                foreach (TempData newData in datas)
                {
                    _dataViewModel.AddDataToSampleRepository(newData);
                }

                MessageBox.Show("نمونه های جدید با موفقیت اضافه شدند", "Report", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                _anythingAdded = true;
            }
        }

        private void OtherClocksTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            OtherClocksTextBox.Clear();
        }

        private void RadDataGrid_Deleting(object sender, Telerik.Windows.Controls.GridViewDeletingEventArgs e)
        {
            datas.Remove((TempData)RadDataGrid.SelectedItem);
        }

        private void RadTimeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TypeOfComboBox.SelectedIndex = 0;
        }

        private void RadTimeRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            TypeOfComboBox.SelectedIndex = -1;
        }
    }
}
