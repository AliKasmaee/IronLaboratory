using System;

namespace IronLaboratory
{
    public class Sample
    {
        public Int64 PrimaryId { get; set; }
        public int SampleId { get; set; }
        public string Registration { get; set; }
        public string Material { get; set; }
        public string Experiment { get; set; }
        public string Clock { get; set; }
        public string OnDate { get; set; }
        public string OrderId { get; set; }
        public string DateAndTimeAdded { get; set; } //added
    }
}
