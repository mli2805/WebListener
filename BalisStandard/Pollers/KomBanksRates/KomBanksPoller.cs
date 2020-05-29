using System.Threading.Tasks;
using UtilsLib;

namespace BalisStandard
{
    public class KomBanksPoller
    {
        private IMyLog _logFile;

        public async void Poll(IMyLog logFile)
        {
            _logFile = logFile;

            await Task.Factory.StartNew(() => Poll(new BelgazMobi()));
            await Task.Factory.StartNew(() => Poll(new BibExtractor()));
            await Task.Factory.StartNew(() => Poll(new PriorExtractor()));
            await Task.Factory.StartNew(() => Poll(new DabrabytExtractor()));
            await Task.Factory.StartNew(() => Poll(new BpsExtractor()));
        }

        private async void Poll(IRatesLineExtractor ratesLineExtractor)
        {
            _logFile.AppendLine($"{ratesLineExtractor.BankTitle} экстрактор запущен");
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