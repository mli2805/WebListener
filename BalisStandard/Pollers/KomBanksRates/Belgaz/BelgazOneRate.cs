namespace BalisStandard
{
    public class BelgazOneRate
    {
        public int B { get; set; }
        public string T { get; set; }  // operation
        public long D { get; set; } // timestamp
        public string C { get; set; } // currency
        public string Bc { get; set; } // currency
        public int Lb { get; set; } // rate bounds
        public object Ub { get; set; } // rate bounds
        public string Rc { get; set; }
        public int Q { get; set; }
        public double V { get; set; } // value
        public int U { get; set; } // unit
        public double Dt { get; set; } // delta
    }
}