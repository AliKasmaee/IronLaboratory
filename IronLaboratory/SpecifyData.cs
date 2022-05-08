using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IronLaboratory
{
    public static class SpecifyData
    {
        public static ObservableCollection<TempSample> SpecificSamples(int x, ObservableCollection<TempData> temps)
        {
            ObservableCollection<TempSample> samples = new ObservableCollection<TempSample>();
            string dateOfToday = DateConversion.ConvertToPersian(DateTime.Today);
            DateTime gregorianDate = DateTime.Today;

            List<TempData> DataList =
                (from temp in temps
                 where temp.TypeOf == x
                 select temp).ToList();

            if (x == 100)
            {
                foreach (TempData i in DataList)
                {
                    TempSample ts = new TempSample();
                    ts.DataId = i.DataId;
                    ts.IsSelected = false;
                    ts.Material = i.Material;
                    ts.Experiment = i.Experiment;
                    ts.Clock = i.Clock;
                    ts.OnDate = dateOfToday;
                    ts.GreDate = gregorianDate;
                    ts.Number = i.Number;
                    ts.OnHour = i.OnHour;
                    ts.OnShift = i.OnShift;

                    samples.Add(ts);
                }

                if (DateTime.Now.DayOfWeek != DayOfWeek.Tuesday)
                {
                    bool exists = samples.Any(e => e.Material == "آهک هیدراته");
                    if (exists)
                    {
                        samples.Remove(samples.Where(i => i.Material == "آهک هیدراته").Single());
                    }
                }

                SetDate(samples);
            }
            else
            {
                foreach (TempData i in DataList)
                {
                    TempSample ts = new TempSample();
                    ts.DataId = i.DataId;
                    ts.IsSelected = false;
                    ts.Material = i.Material;
                    ts.Experiment = i.Experiment;
                    ts.Clock = i.Clock;
                    ts.OnDate = dateOfToday;
                    ts.GreDate = gregorianDate;
                    ts.Number = i.Number;

                    samples.Add(ts);
                }
            }

            return samples;
        }

        private static void SetDate(ObservableCollection<TempSample> tempSamples)
        {
            DateTime dateTime = DateTime.Now;
            double currentHour = dateTime.TimeOfDay.TotalHours;
            string dateOfTomorrow = DateConversion.ConvertToPersian(DateTime.Today.AddDays(1));

            foreach (TempSample sample in tempSamples)
            {
                if (currentHour > (double)sample.OnHour)
                {
                    sample.OnDate = dateOfTomorrow;
                    sample.OnHour += 24;
                }
            }
        }
    }
}
