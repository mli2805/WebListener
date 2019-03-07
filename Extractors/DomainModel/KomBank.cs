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
        Bgpb, Bib, Mmb, Bps, Prior,
    }

    public static class KomBankExt
    {
        public static string GetAbbreviation(this KomBank2 komBank)
        {
            switch (komBank)
            {
                case KomBank2.Bgpb:  return "БГПБ";
                case KomBank2.Bib:   return "БИБ";
                case KomBank2.Mmb:   return "ММБ";
                case KomBank2.Bps:   return "БПС";
                case KomBank2.Prior: return "Приор";
            }

            return "";
        }

        public static string GetFilename(this KomBank2 komBank2)
        {
            switch (komBank2)
            {
                case KomBank2.Bgpb:  return @"data\mmbank.txt";
                case KomBank2.Bib:   return @"data\bgpbank.txt";
                case KomBank2.Mmb:   return @"data\bibank.txt";
                case KomBank2.Bps:   return @"data\bpsbank.txt";
                case KomBank2.Prior: return @"data\priorbank.txt";
                default:
                    return "unknown.txt";
            }
        }

    }
}
