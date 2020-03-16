using System;
using System.Collections.Generic;

namespace BalisStandard
{
    // https://jsoneditoronline.org/
    // не забывать обновлять правую сторону
    public class BankUnit
    {
        public int Code { get; set; }
        public int Region { get; set; }
        public long City { get; set; }
        public int District { get; set; }
    }

    public class Rate
    {
        public int QuantityFrom { get; set; }
        public int QuantityTo { get; set; }
        public double SellRate { get; set; }
        public double BuyRate { get; set; }
     
    }

    public class ViewVoList
    {
        public int Channel { get; set; }
        public BankUnit BankUnit { get; set; }
        public DateTime ValidFrom { get; set; }
        public int BaseCurrency { get; set; }
        public int BaseCurrencyNominal { get; set; }
        public int BaseCurrencyPayType { get; set; }
        public int RatedCurrency { get; set; }
        public int RatedCurrencyPayType { get; set; }
        public Rate Rate { get; set; }
      
    }

    public class Meta
    {
        public int Offset { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
        public DateTime ProcessingDate { get; set; }

    }


    public class CalculatorTable
    {
        public Meta Meta { get; set; }
        public IList<ViewVoList> Data { get; set; }
    }

    public class RootObject
    {
        public string Message { get; set; }
        public CalculatorTable CalculatorTable { get; set; }
    }

}
