﻿using System;
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
            _logFile.AppendLine("Used to check in browser. Type   http://localhost:11082/bali   ");
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);

            var result = new List<KomBankRatesLine>();
            foreach (var komBank in (KomBankE[])Enum.GetValues(typeof(KomBankE)))
            {
                result.Add(await db.KomBankRates
                    .Where(r => r.Bank == komBank.ToString().ToUpper())
                    .OrderByDescending(l => l.LastCheck)
                    .FirstOrDefaultAsync());
            }
            return result;
        }

        [HttpGet("get-some-last/{bankTitle}")]
        public async Task<List<KomBankRatesLine>> GetSomeLast(string bankTitle)
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            return db.KomBankRates
                .Where(r => r.Bank == bankTitle.ToUpper() && r.StartedFrom > DateTime.Today.AddDays(-7))
                .OrderByDescending(l => l.StartedFrom)
                .ToList();
        }

        // http://192.168.96.19:11082/bali/get-some-last-days-for-bank?bankTitle=BIB&days=3
        [HttpGet("get-some-last-days-for-bank")]
        public async Task<List<KomBankRatesLine>> GetSomeLastDaysForBank(string bankTitle, int days)
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            return db.KomBankRates
                .Where(r => r.Bank == bankTitle.ToUpper() && r.StartedFrom > DateTime.Today.AddDays(-days))
                .OrderByDescending(l => l.StartedFrom)
                .ToList();
        }

        [HttpGet("get-one-last/{bankTitle}")]
        public async Task<KomBankRatesLine> GetLast(string bankTitle)
        {
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);

            return db.KomBankRates
                .OrderByDescending(l => l.LastCheck)
                .FirstOrDefault(r => r.Bank == bankTitle.ToUpper());
        }

        [HttpGet("get-banki24-archive/{portion}")]
        public async Task<List<BelStockArchiveOneCurrency>> GetBanki24Archive(int portion)
        {
            int portionSize = _iniFile.Read(IniSection.General, IniKey.BelstockPortionSize, 100);
            await using BanksListenerContext db = new BanksListenerContext(_dbPath);
            return db.Banki24Archive.OrderBy(l => l.Date)
                .Skip(portion * portionSize)
                .Take(portionSize)
                .ToList();
        }


    }
}