using System.Threading.Tasks;

namespace BalisLibrary
{
    public interface IRatesLineExtractor
    {
        Task<KomBankRatesLine> GetRatesLineAsync();
    }
}