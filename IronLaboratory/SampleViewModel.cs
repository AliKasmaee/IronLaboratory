using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace IronLaboratory
{
    public class SampleViewModel
    {
        public ObservableCollection<Sample> Samples { get; set; }
        private SampleRepository SamplesRepository { get; set; }
        public SampleViewModel()
        {
            SamplesRepository = new SampleRepository();
            Samples = new ObservableCollection<Sample>(SamplesRepository.Repository);
            Samples.CollectionChanged += Samples_CollectionChanged;
        }

        public int AddDataTableToSampleRepository(DataTable dt)
        {
            int inserted = 0;
            foreach (DataRow row in dt.Rows)
            {
                Sample newSample = new Sample()
                {
                    SampleId = SampleRepository.SampleID,
                    Registration = row["Registration"].ToString(),
                    Material = row["Material"].ToString(),
                    Experiment = row["Experiment"].ToString(),
                    Clock = row["Clock"].ToString(),
                    OnDate = DateConversion.ConvertToGregorian(row["OnDate"].ToString()),
                    OrderId = row["OrderID"].ToString()
                };
                Samples.Add(newSample);
                SampleRepository.SampleID += 1;
                inserted++;

                //Although ResetSampleID() itself checks for SampleID == 5001, SampleID is checked here in order not to call function multiple times
                if (SampleRepository.SampleID == 5001)
                {
                    CheckSampleID.ResetSampleID();
                }
            }
            return inserted;
        }

        public void DeleteRecordFromRepository(Int64 primaryId)
        {
            if (primaryId <= 0)
            {
                throw new Exception("Sample ID can not be negative");
            }

            int index = 0;
            while (index < Samples.Count)
            {
                if (Samples[index].PrimaryId == primaryId)
                {
                    Samples.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public void UpdateRecordInRepository(Sample sample)
        {
            if (sample.PrimaryId <= 0)
            {
                throw new Exception("Sample ID can not be negative");
            }

            int index = 0;
            while (index < Samples.Count)
            {
                if (Samples[index].PrimaryId == sample.PrimaryId)
                {
                    Samples[index] = sample;
                    break;
                }
                index++;
            }
        }

        private void Samples_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                int newIndex = e.NewStartingIndex;
                SamplesRepository.AddNewSample(Samples[newIndex]);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                List<Sample> temp = e.OldItems.OfType<Sample>().ToList();
                SamplesRepository.DeleteRecord(temp[0].PrimaryId);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                List<Sample> temp = e.NewItems.OfType<Sample>().ToList();
                SamplesRepository.UpdateRecord(temp[0]);
            }
        }
    }
}
