using System;

namespace IronLaboratory
{
    public class TempData
    {
        public int DataId { get; set; }
        public string Material { get; set; }
        public string Experiment { get; set; }
        public string Clock { get; set; }
        public Int16 Number { get; set; } //added
        public Int16 OnHour { get; set; } //added
        public string OnShift { get; set; } //added
        public Int16 TypeOf { get; set; } //added
        public string TypeOfSt { get; set; } //added, (Not to be saved to database)
    }
}
