using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace BalisStandard
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BuyRate
    {
        public string caption { get; set; }
        public string value { get; set; }
        public int dynamics { get; set; }
    }

    public class BelVebCurrency3
    {
        public string currency_img { get; set; }
        public string currency_qty { get; set; }
        public string currency_cod { get; set; }
        public string currency_name { get; set; }
        public string time { get; set; }
        public BuyRate buy_rate { get; set; }
        public SaleRate sale_rate { get; set; }
        public string type { get; set; }
    }

    public class BelVebRoot3
    {
        public List<BelVebTime> time { get; set; }
        public List<BelVebCurrency3> currency { get; set; }
    }

    public class SaleRate
    {
        public string caption { get; set; }
        public string value { get; set; }
        public int dynamics { get; set; }
    }

    public class BelVebTime
    {
        public string value { get; set; }
    }


}
