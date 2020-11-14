using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BalisStandard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtilsLib;

namespace BanksListener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaliController : ControllerBase
    {
        private readonly IniFile _iniFile;
        private readonly IMyLog _logFile;
        private readonly string _dbPath;

        public BaliController(IniFile iniFile, IMyLog logFile)
        {
            _iniFile = iniFile;
            _logFile = logFile;
            _dbPath = iniFile.Read(IniSection.Sqlite, IniKey.DbPath, @"..\bali.db");
        }

        [HttpGet]
        public async Task<List<KomBankRatesLine>> Get()
        {
            _logFile.AppendLine("request received");
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);

            var result = new List<KomBankRatesLine>();
            result.Add(await db.KomBankRates.Where(r => r.Bank == "BGPB").OrderByDescending(l => l.LastCheck).FirstOrDefaultAsync());
            result.Add(await db.KomBankRates.Where(r => r.Bank == "BIB").OrderByDescending(l => l.LastCheck).FirstOrDefaultAsync());
            return result;
        }

        [HttpGet("get-some-last/{bankTitle}")]
        public async Task<List<KomBankRatesLine>> GetSomeLast(string bankTitle)
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            return db.KomBankRates
                .Where(r => r.Bank == bankTitle.ToUpper())
                .OrderByDescending(l => l.LastCheck)
                .Take(13)
                .ToList();
        }

        [HttpGet("get-last-for-every}")]
        public async Task<List<KomBankRatesLine>> GetLastForEvery()
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            var result = new List<KomBankRatesLine>();
            foreach (KomBankE komBank in Enum.GetValues(typeof(KomBankE)))
            {
                var lastLine = db.KomBankRates
                    .OrderByDescending(l => l.LastCheck)
                    .FirstOrDefault(r => r.Bank == komBank.ToString());
                if (lastLine != null)
                    result.Add(lastLine);
            }
            return result;
        }

        [HttpGet("get-belstock-archive/{portion}")]
        public async Task<List<BelStockArchiveOneCurrencyDay>> GetBelstockArchive(int portion)
        {
            int portionSize = _iniFile.Read(IniSection.General, IniKey.BelstockPortionSize, 100);
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            return db.BelStockArchive.OrderBy(l => l.Date)
                .Skip(portion * portionSize)
                .Take(portionSize)
                .ToList();
        }


    }
}