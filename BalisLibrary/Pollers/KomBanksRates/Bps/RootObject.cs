using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace BalisLibrary
{
    public class List
    {
        public string rateType { get; set; }
        public string iso { get; set; }
        public object iso2 { get; set; }
        public double buy { get; set; }
        public double deltaBuy { get; set; }
        public double sale { get; set; }
        public double deltaSale { get; set; }
        public object minsum { get; set; }
        public object maxsum { get; set; }
        public int? scale { get; set; }
        public object rate { get; set; }
        public object deltaRate { get; set; }
    }

    public class Rates
    {
        public List<List> list { get; set; }
    }

    public class ErrorInfo
    {
        public string errorCode { get; set; }
        public object errorDescription { get; set; }
    }

    public class BpsRootObject
    {
        public long date { get; set; }
        public long prevDate { get; set; }
        public object nextDate { get; set; }
        public string idBranch { get; set; }
        public object branches { get; set; }
        public Rates rates { get; set; }
        public ErrorInfo errorInfo { get; set; }
        public int typeId { get; set; }
    }
}
