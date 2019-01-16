using System;

namespace WebListener.DomainModel.BelStock
{
    public class BelStockArchiveDay
    {
        public DateTime Date { get; set; }
        public BelStockArchiveOneCurrencyDay Usd { get; set; }
        public BelStockArchiveOneCurrencyDay Eur { get; set; }
        public BelStockArchiveOneCurrencyDay Rub { get; set; }

        public double TurnoverEurInUsd => Math.Round(Eur.TurnoverInByn / Usd.Average, 1);
        public double TurnoverRubInUsd => Math.Round(Rub.TurnoverInByn / Usd.Average, 1);
        public double TotalTurnover => Math.Round(Usd.TurnoverInCurrency + TurnoverEurInUsd + TurnoverRubInUsd);


        public BelStockArchiveDay()
        {
            
        }
        public BelStockArchiveDay(string str)
        {
            var posUsd = str.IndexOf("Usd", StringComparison.Ordinal);
            Date = DateTime.Parse(str.Substring(0, posUsd).Trim());
            var posEur = str.IndexOf("Eur", StringComparison.Ordinal);
            Usd = new BelStockArchiveOneCurrencyDay(str.Substring(posUsd, posEur - posUsd));
            var posRub = str.IndexOf("Rub", StringComparison.Ordinal);
            Eur = new BelStockArchiveOneCurrencyDay(str.Substring(posEur, posRub - posEur));
            Rub = new BelStockArchiveOneCurrencyDay(str.Substring(posRub));
        }

        public string ToFileString()
        {
            return
                $"{Date:dd-MM-yyyy} Usd {(Usd == null ? "" : Usd.ToFileString())} Eur {(Eur == null ? "" : Eur.ToFileString())} Rub {(Rub == null ? "" : Rub.ToFileString())}";
        }
    }
}