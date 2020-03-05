using System.Threading.Tasks;

namespace BanksListener
{
    public interface IRatesLineExtractor
    {
        Task<KomBankRatesLine> GetRatesLineAsync();
    }
}