using System.Threading.Tasks;

namespace BalisStandard
{
    public interface IRatesLineExtractor
    {
        Task<KomBankRatesLine> GetRatesLineAsync();
    }
}