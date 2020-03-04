using System;
using System.Threading.Tasks;

namespace ConsCoreFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Fetch();
            Console.ReadKey();
        }

        private static async void Fetch()
        {
            var belgaz = new BelgazMobi();

            using (BanksListenerContext db = new BanksListenerContext())
            {
                KomBankRatesLine rate;
                while (true)
                {
                    rate = await belgaz.GetRatesLineAsync();
                    if (rate != null) break;
                    await Task.Delay(2000);
                }

                db.KomBankRates.Add(rate);
                await db.SaveChangesAsync();
            }

        }
    }
}
