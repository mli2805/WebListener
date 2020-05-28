namespace BalisStandard
{
    public class TradingViewRates
    {
        public TikerValues EurUsd { get; set; } = new TikerValues();
        public TikerValues UsdRub { get; set; } = new TikerValues();
        public TikerValues EurRub { get; set; } = new TikerValues();
        public TikerValues UkOil { get; set; } = new TikerValues();
        public TikerValues Gold { get; set; } = new TikerValues();
        public TikerValues SpSpx { get; set; } = new TikerValues();
        public TikerValues AmexVoo { get; set; } = new TikerValues();
        public TikerValues CboeVix { get; set; } = new TikerValues();
        public TikerValues AmexBnd { get; set; } = new TikerValues();
    }
}