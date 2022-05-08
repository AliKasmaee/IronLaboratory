using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IronLaboratory
{
    public class DataViewModel
    {
        public ObservableCollection<TempData> Datas { get; set; }
        private DataRepository DatasRepository { get; set; }
        public DataViewModel()
        {
            DatasRepository = new DataRepository();
            Datas = new ObservableCollection<TempData>(DatasRepository.Repository);
            Datas.CollectionChanged += Datas_CollectionChanged;
        }

        public void AddDataToSampleRepository(TempData td)
        {
            if (td == null)
            {
                throw new ArgumentNullException("Error: The argument is Null");
            }

            Datas.Add(td);
        }

        public void DeleteDataFromRepository(int dId)
        {
            if (dId <= 0)
            {
                throw new Exception("ID can not be negative");
            }

            int index = 0;
            while (index < Datas.Count)
            {
                if (Datas[index].DataId == dId)
                {
                    Datas.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                int newIndex = e.NewStartingIndex;
                DatasRepository.AddNewData(Datas[newIndex]);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                List<TempData> temp = e.OldItems.OfType<TempData>().ToList();
                DatasRepository.DeleteData(temp[0].DataId);
            }
        }
    }
}
