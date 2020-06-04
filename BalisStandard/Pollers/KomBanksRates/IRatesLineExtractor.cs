using System.Threading.Tasks;

namespace BalisStandard
{
    public interface IRatesLineExtractor
    {
        string BankTitle { get; } 
        Task<KomBankRatesLine> GetRatesLineAsync();
    }
}