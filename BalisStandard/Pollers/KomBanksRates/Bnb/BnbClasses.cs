// ReSharper disable InconsistentNaming
namespace BalisStandard
{
   
    public class ImBankingCurrency    {
        public double UF_BUY { get; set; } 
        public double UF_SALE { get; set; } 
        public string UF_CURRENCY { get; set; } 
        public int UF_TYPE { get; set; } 
        public int COUNT { get; set; } 
        public string date { get; set; } 
        public int sign_buy { get; set; } 
        public int sign_sale { get; set; } 
    }

    public class BnbCurrency    {
        public string UF_CURRENCY { get; set; } 
        public double UF_BUY { get; set; } 
        public double UF_SALE { get; set; } 
        public string UF_TYPE { get; set; } 
        public string SORT { get; set; } 
        public object UF_DATE { get; set; } 
        public string UF_OTDELENIE { get; set; } 
    }

}
