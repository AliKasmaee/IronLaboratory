using System;

namespace IronLaboratory
{
    public class TempSample
    {
        public int DataId { get; set; } //added, (assigned by database)
        public bool IsSelected { get; set; } //added, (assigned by app)
        public string Registration { get; set; }
        public string Material { get; set; } //(assigned by database)
        public string Experiment { get; set; } //(assigned by database)
        public string Clock { get; set; } //(assigned by database)
        public string OnDate { get; set; } //(assigned by app)
        public DateTime GreDate { get; set; } //added, (assigned by app)
        public string OrderId { get; set; }
        public int Number { get; set; } //added, (assigned by database)
        public int OnHour { get; set; } //added, (assigned by database)
        public string OnShift { get; set; } //added, (assigned by database)
        public int TypeOf { get; set; } //added, (assigned by database)
    }
}
