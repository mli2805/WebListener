using System;
using Caliburn.Micro;

namespace WebListener
{
    public class TradingViewVm : PropertyChangedBase
    {
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (value.Equals(_timestamp)) return;
                _timestamp = value;
                NotifyOfPropertyChange();
            }
        }

        private string _eurUsdRate;
        public string EurUsdRate
        {
            get { return _eurUsdRate; }
            set
            {
                if (_eurUsdRate == value) return;
                _eurUsdRate = value;
                NotifyOfPropertyChange();
            }
        }

        private double _usdRub;
        public double UsdRub
        {
            get { return _usdRub; }
            set
            {
                _usdRub = value;
                UsdRubRate = _usdRub.ToString("0.0000");
            }
        }

        private string _usdRubRate;
        private string _eurRubRate;

        public string UsdRubRate
        {
            get { return _usdRubRate; }
            set
            {
                if (value == _usdRubRate) return;
                _usdRubRate = value;
                NotifyOfPropertyChange();
            }
        }

        public string EurRubRate
        {
            get { return _eurRubRate; }
            set
            {
                if (value == _eurRubRate) return;
                _eurRubRate = value;
                NotifyOfPropertyChange();
            }
        }

        private double _brent;
        public double Brent
        {
            get { return _brent; }
            set
            {
                _brent = value;
                BrentStr = Brent.ToString("0.000");
            }
        }

        private string _brentStr;

        public string BrentStr
        {
            get { return _brentStr; }
            set
            {
                if (value == _brentStr) return;
                _brentStr = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(Bochka));
            }
        }

        public string Bochka => (Brent * _usdRub).ToString("#");
    }
}