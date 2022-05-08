using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for AddPage2.xaml
    /// </summary>
    public partial class AddPage2 : Page
    {
        Frame _frame;
        SampleViewModel _sampleViewModel;
        DataViewModel _dataViewModel;
        DataTable _dataTable;

        public AddPage2()
        {
            InitializeComponent();
        }

        public AddPage2(Frame f, SampleViewModel svm, DataViewModel dvm)
        {
            InitializeComponent();
            _frame = f;
            _sampleViewModel = svm;
            _dataViewModel = dvm;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void UpdateDataGridSource()
        {
            RadDataGrid.ItemsSource = SpecifyData.SpecificSamples(200, _dataViewModel.Datas);
        }

        private void RadDataGrid_PreparingCellForEdit(object sender, Telerik.Windows.Controls.GridViewPreparingCellForEditEventArgs e)
        {
            var currentRow = e.Row;
            currentRow.IsSelected = true;

            if ((string)e.Column.Header == "تاریخ")
            {
                var tb = e.EditingElement as RadDatePicker;
                tb.Culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            }
        }

        private void RadDataGrid_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            TempSample temp = e.Cell.DataContext as TempSample;
            temp.OnDate = DateConversion.ConvertToPersian(temp.GreDate);
        }

        private void RadDataGrid_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            CountTextBox.Text = RadDataGrid.SelectedItems.Count.ToString();
            var selectedItems = e.AddedItems;
            var unselectedItems = e.RemovedItems;

            foreach (var item in selectedItems)
            {
                TempSample temp = (TempSample)item;
                temp.IsSelected = true;
            }
            foreach (var item in unselectedItems)
            {
                TempSample temp = (TempSample)item;
                temp.IsSelected = false;
            }
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else if (e.Key == Key.Delete)
            {
                RadDataGrid.CommitEdit();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.NavigationService.GoBack();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGridSource();
            RadDataGrid.SelectedItems.Clear();
            CountTextBox.Text = RadDataGrid.SelectedItems.Count.ToString();
            DatePicker.Culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            DatePicker.SelectedDate = null;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            RadDataGrid.PendingCommands.Add(RadGridViewCommands.CommitEdit);
            RadDataGrid.ExecutePendingCommand();
            List<TempSample> selectedList = new List<TempSample>();
            var allSamples = new List<TempSample>(RadDataGrid.ItemsSource as IEnumerable<TempSample>);
            _dataTable = ConvertToDataTable.ToDataTable(allSamples);

            List<int> numberList = new List<int>();

            foreach (DataRow row in _dataTable.Rows)
            {
                bool check = Convert.ToBoolean(row["IsSelected"]);
                if (check)
                {
                    selectedList.Add(new TempSample { Registration = row["Registration"].ToString(), Material = row["Material"].ToString(), Experiment = row["Experiment"].ToString(), Clock = row["Clock"].ToString(), OnDate = row["OnDate"].ToString(), OrderId = row["OrderID"].ToString(), Number = Convert.ToInt32(row["Number"]) });
                }
            }

            foreach (TempSample item in selectedList)
            {
                numberList.Add(item.Number);
            }

            if (selectedList.Count == 0)
            {
                MessageBox.Show("ابتدا نمونه ها را انتخاب کنید*", null, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
            else if (selectedList.Count > 0)
            {
                DataTable selectedDataTable = ConvertToDataTable.ToDataTable(selectedList);
                int numberOfInserted = _sampleViewModel.AddDataTableToSampleRepository(selectedDataTable);

                var result = MessageBox.Show("تعداد نمونه های انتخاب شده: " + selectedList.Count + "\n" + "تعداد نمونه های اضافه شده: " + numberOfInserted + "\n" + "------------------------------" + "\n" + "\n" + "نمونه ها چاپ شوند؟", "Report", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);

                if (result == MessageBoxResult.Yes)
                {
                    //_frame.NavigationService.Navigate(new PrintPage(selectedList.Count, _frame, _sampleViewModel, numberList));
                    PrintWindow printWindow = new PrintWindow(_sampleViewModel, selectedList.Count, numberList);
                    printWindow.Show();
                }
            }
        }

        private void DatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var allSamples = new List<TempSample>(RadDataGrid.ItemsSource as IEnumerable<TempSample>);
            if (!DatePicker.SelectedDate.HasValue)
            {
                return;
            }
            else
            {
                foreach (TempSample t in allSamples)
                {
                    t.GreDate = (DateTime)DatePicker.SelectedDate;
                    t.OnDate = DateConversion.ConvertToPersian(t.GreDate);
                }

                RadDataGrid.Rebind();
            }
        }

        private void RadDataGrid_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            IEnumerable<object> _itemsToBeDeleted = e.Items;

            var result = MessageBox.Show("آیا از حذف نمونه ها مطمئن هستید؟", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RightAlign);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else if (result == MessageBoxResult.Yes)
            {
                foreach (TempSample ts in _itemsToBeDeleted)
                {
                    RadDataGrid.Items.Remove(ts);
                    _dataViewModel.DeleteDataFromRepository(ts.DataId);
                }
            }
        }
    }
}
