﻿namespace BalisStandard
{
    public enum KomBankE
    {
        Bgpb, Bib, Mmb, Bps, Prior, Bveb, Alfa, Mtb, Bnb, Vtb
    }

    public static class KomBankExt
    {
        public static string GetAbbreviation(this KomBankE komBank)
        {
            switch (komBank)
            {
                case KomBankE.Bgpb: return "БГПБ";
                case KomBankE.Bib: return "БИБ";
                case KomBankE.Mmb: return "ММБ";
                case KomBankE.Bps: return "БПС";
                case KomBankE.Prior: return "Приор";
                case KomBankE.Bveb: return "БелВЭБ";
                case KomBankE.Alfa: return "Альфа";
                case KomBankE.Mtb: return "МТБ";
                case KomBankE.Bnb: return "БНБ";
                case KomBankE.Vtb: return "ВТБ";
            }

            return "";
        }
    }
}
