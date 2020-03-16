namespace BalisStandard
{
    public enum KomBankE
    {
        Bgpb, Bib, Mmb, Bps, Prior, Bveb
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
            }

            return "";
        }

       public static KomBankE? ToKomBank2(this string title)
        {
            switch (title)
            {
                case "BGPB"  : return KomBankE.Bgpb   ;
                case "BIB"   : return KomBankE.Bib  ;
                case "MMB"   : return KomBankE.Mmb  ;
                case "BPS"   : return KomBankE.Bps  ;
                case "Prior" : return KomBankE.Prior;
                case "BelVEB": return KomBankE.Bveb ;
            }

            return null;
        }

        public static string GetFilename(this KomBankE komBankE)
        {
            switch (komBankE)
            {
                case KomBankE.Mmb: return @"data\mmbank.txt";
                case KomBankE.Bgpb: return @"data\bgpbank.txt";
                case KomBankE.Bib: return @"data\bibank.txt";
                case KomBankE.Bps: return @"data\bpsbank.txt";
                case KomBankE.Prior: return @"data\priorbank.txt";
                default:
                    return "unknown.txt";
            }
        }

    }
}
