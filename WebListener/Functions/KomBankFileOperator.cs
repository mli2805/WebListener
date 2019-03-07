﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Extractors;

namespace WebListener
{
    class KomBankFileOperator
    {
        private string GetFilename(string bankname)
        {
            switch (bankname)
            {
                case "ММБ": return @"mmbank.txt";
                case "БГПБ": return @"bgpbank.txt";
                case "БИБ": return @"bibank.txt";
                case "БПС": return @"bpsbank.txt";
                case "Приор": return @"priorbank.txt";
                default:
                    return "unknown.txt";
            }
        }
        public void SaveLine(KomBankRates currentLine)
        {
            var filename = GetFilename(currentLine.Bank);
            var content = new List<string> { currentLine.ToFileString() };
            try
            {
                File.AppendAllLines(filename, content);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} during SaveLine");
                MessageBox.Show($"{e.Message} during SaveLine");
            }
        }

        public void ReWriteSet(ObservableCollection<KomBankRates> set)
        {
            var filename = GetFilename(set.First().Bank);
            var content = set.Select(line => line.ToFileString()).ToList();

            try
            {
                File.WriteAllLines(filename + ".new", content);
                File.Replace(filename + ".new", filename, filename + ".bak");
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} during ReWriteSet");
                MessageBox.Show($"{e.Message} during ReWriteSet");
            }
        }

        private ObservableCollection<KomBankRates> LoadLines(string bankname)
        {
            var result = new ObservableCollection<KomBankRates>();
            var filename = GetFilename(bankname);
            if (!File.Exists(filename)) return result;
            var content = File.ReadAllLines(filename);
            foreach (var line in content)
            {
                result.Add(new KomBankRates(line));
            }
            return result;
        }
        public void LoadBanksHistory(MainViewModel vm)
        {
            vm.RowsMoMi = LoadLines("ММБ");
            vm.RowsBelGaz = LoadLines("БГПБ");
            vm.RowsBib = LoadLines("БИБ");
            vm.RowsBps = LoadLines("БПС");
            vm.RowsPrior = LoadLines("Приор");
        }

        public void SaveForex(Forex line)
        {
            var content = new List<string> { line.ToFileString() };
            try
            {
                File.AppendAllLines(@"forex.txt", content);
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message} during SaveForex");
                MessageBox.Show($"{e.Message} during SaveForex");
            }
        }
    }
}
