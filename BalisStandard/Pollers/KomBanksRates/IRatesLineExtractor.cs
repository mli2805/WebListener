using System.Threading.Tasks;

namespace BalisStandard
{
    public interface IRatesLineExtractor
    {
        string BankTitle { get; set; } 
        Task<KomBankRatesLine> GetRatesLineAsync();
    }
}