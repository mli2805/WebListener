using System;
using System.Text;

namespace BalisStandard
{
    public static class TikerExt
    {
        private const string EurUsdRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURUSD\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string UsdRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:USDRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string EurRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:EURRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string BrentRequest = "{\"p\":[\"my_session\",\"FX:UKOIL\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string GoldRequest = "{\"p\":[\"my_session\",\"FX:XAUUSD\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string VooRequest = "{\"p\":[\"my_session\",\"AMEX:VOO\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string BndRequest = "{\"p\":[\"my_session\",\"NASDAQ:BND\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string VixRequest = "{\"p\":[\"my_session\",\"CBOE:VIX\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string SpxRequest = "{\"p\":[\"my_session\",\"SP:SPX\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string UsdCnyRequest = "{\"p\":[\"my_session\",\"FX_IDC:USDCNY\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";
        private const string CnyRubRequest = "{\"p\":[\"my_session\",\"FX_IDC:CNYRUB\",{\"flags\":[\"force_permission\"]}],\"m\":\"quote_add_symbols\"}";

        private static string TikerToRequest(this TradingViewTiker tiker)
        {
            switch (tiker)
            {
                case TradingViewTiker.EurUsd: return EurUsdRequest;
                case TradingViewTiker.UsdRub: return UsdRubRequest;
                case TradingViewTiker.EurRub: return EurRubRequest;
                case TradingViewTiker.UkOil: return BrentRequest;
                case TradingViewTiker.Gold: return GoldRequest;
                case TradingViewTiker.Voo: return VooRequest;
                case TradingViewTiker.Vix: return VixRequest;
                case TradingViewTiker.Spx: return SpxRequest;
                case TradingViewTiker.Bnd: return BndRequest;
                case TradingViewTiker.UsdCny: return UsdCnyRequest;
                case TradingViewTiker.CnyRub: return CnyRubRequest;
                default: return UsdRubRequest;
            }
        }

        public static ArraySegment<byte> ToBufferizedRequest(this TradingViewTiker tiker)
        {
            var request = tiker.TikerToRequest();
            var framed = $"~m~{request.Length}~m~{request}";
            var bytes = Encoding.UTF8.GetBytes(framed);
            return new ArraySegment<byte>(bytes, 0, bytes.Length);
        }

    }
}