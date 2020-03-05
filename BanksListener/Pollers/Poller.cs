using System.Threading.Tasks;

namespace BanksListener
{
    public class Poller
    {
        public async void Poll()
        {
            await Task.Factory.StartNew(() => Poll(new BelgazMobi()));
            await Task.Factory.StartNew(() => Poll(new BibExtractor()));
        }

        private async void Poll(IRatesLineExtractor ratesLineExtractor)
        {
            while (true)
            {
                KomBankRatesLine rate;
                while (true)
                {
                    rate = await ratesLineExtractor.GetRatesLineAsync();
                    if (rate != null) break;
                    await Task.Delay(2000);
                }

                var unused = await rate.Persist();
                await Task.Delay(15000);

            }
            // ReSharper disable once FunctionNeverReturns
        }
     
    }
}