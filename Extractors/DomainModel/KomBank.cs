namespace Extractors
{
    public enum KomBank
    {
        Бгпб = 1,
        Биб,
        Ммб,
        Бпс,
        Приор,

    }

    public enum KomBank2
    {
        Bgpb, Bib, Mmb, Bps, Prior, Bveb
    }

    public static class KomBankExt
    {
        public static string GetAbbreviation(this KomBank2 komBank)
        {
            switch (komBank)
            {
                case KomBank2.Bgpb: return "БГПБ";
                case KomBank2.Bib: return "БИБ";
                case KomBank2.Mmb: return "ММБ";
                case KomBank2.Bps: return "БПС";
                case KomBank2.Prior: return "Приор";
                case KomBank2.Bveb: return "БелВЭБ";
            }

            return "";
        }

       public static KomBank2? ToKomBank2(this string title)
        {
            switch (title)
            {
                case "BGPB"  : return KomBank2.Bgpb   ;
                case "BIB"   : return KomBank2.Bib  ;
                case "MMB"   : return KomBank2.Mmb  ;
                case "BPS"   : return KomBank2.Bps  ;
                case "Prior" : return KomBank2.Prior;
                case "BelVEB": return KomBank2.Bveb ;
            }

            return null;
        }

        public static string GetFilename(this KomBank2 komBank2)
        {
            switch (komBank2)
            {
                case KomBank2.Mmb: return @"data\mmbank.txt";
                case KomBank2.Bgpb: return @"data\bgpbank.txt";
                case KomBank2.Bib: return @"data\bibank.txt";
                case KomBank2.Bps: return @"data\bpsbank.txt";
                case KomBank2.Prior: return @"data\priorbank.txt";
                default:
                    return "unknown.txt";
            }
        }

    }
}
