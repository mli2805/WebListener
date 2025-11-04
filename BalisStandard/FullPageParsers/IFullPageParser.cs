namespace BalisStandard;

public interface IFullPageParser
{
    KomBankRatesLine? ParseKomBankRatesFromHtml(string html);

}