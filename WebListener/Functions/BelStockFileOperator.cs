using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WebListener.DomainModel.BelStock;

namespace WebListener.Functions
{
    public class BelStockFileOperator
    {
        public void SaveBelStockArchieve(IEnumerable<BelStockArchiveDay> archieve)
        {
            var content = archieve.Select(day => day.ToFileString()).ToList();
            try
            {
                File.WriteAllLines(@"BelStockArchieve.txt", content);

            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} during SaveBelStockArchieve");
                MessageBox.Show($"{e.Message} during SaveBelStockArchieve");
            }
        }

        public IEnumerable<BelStockArchiveDay> LoadBelStockArchieve()
        {
            if (!File.Exists(@"BelStockArchieve.txt")) return Enumerable.Empty<BelStockArchiveDay>();
            var content = File.ReadAllLines(@"BelStockArchieve.txt");
            return (content.Select(line => new BelStockArchiveDay(line)));
        }
    }
}