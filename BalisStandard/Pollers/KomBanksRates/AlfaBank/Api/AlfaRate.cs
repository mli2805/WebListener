using System;
// ReSharper disable InconsistentNaming

namespace BalisStandard
{
    public class AlfaRate
    {
        public double sellRate { get; set; }
        public string sellIso { get; set; }
        public int sellCode { get; set; }
        public double buyRate { get; set; }
        public string buyIso { get; set; }
        public int buyCode { get; set; }
        public int quantity { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
    }
}