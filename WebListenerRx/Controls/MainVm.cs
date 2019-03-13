using System.Linq;
using Caliburn.Micro;
using Extractors;

namespace WebListenerRx
{
    public class MainVm : PropertyChangedBase
    {
        private OneBankViewModel _bibModel = new OneBankViewModel();
        private OneBankViewModel _priorModel = new OneBankViewModel();
        private OneBankViewModel _mmbModel = new OneBankViewModel();
        private OneBankViewModel _bpsModel = new OneBankViewModel();
        private OneBankViewModel _bgpbModel = new OneBankViewModel();

        public OneBankViewModel BibModel
        {
            get => _bibModel;
            set
            {
                if (Equals(value, _bibModel)) return;
                _bibModel = value;
                NotifyOfPropertyChange();
            }
        }

        public OneBankViewModel PriorModel
        {
            get => _priorModel;
            set
            {
                if (Equals(value, _priorModel)) return;
                _priorModel = value;
                NotifyOfPropertyChange();
            }
        }

        public OneBankViewModel MmbModel
        {
            get => _mmbModel;
            set
            {
                if (Equals(value, _mmbModel)) return;
                _mmbModel = value;
                NotifyOfPropertyChange();
            }
        }

        public OneBankViewModel BpsModel
        {
            get => _bpsModel;
            set
            {
                if (Equals(value, _bpsModel)) return;
                _bpsModel = value;
                NotifyOfPropertyChange();
            }
        }

        public OneBankViewModel BgpbModel
        {
            get => _bgpbModel;
            set
            {
                if (Equals(value, _bgpbModel)) return;
                _bgpbModel = value;
                NotifyOfPropertyChange();
            }
        }

        private int _countOfReady;
        private bool _allBanksLoaded;

        public bool AllBanksLoaded
        {
            get => _allBanksLoaded;
            set
            {
                if (value == _allBanksLoaded) return;
                _allBanksLoaded = value;
                NotifyOfPropertyChange();
            }
        }

        public void AssignModel(KomBank2 komBank2, OneBankViewModel model)
        {
            var t = model.Rows.Last().Clone();
            switch (komBank2)
            {
                case KomBank2.Bib: 
                    BibModel = model; 
                    BibModel.Rows.Add(t);
                    BibModel.Rows.Remove(t);
                    break;
                case KomBank2.Prior: 
                    PriorModel = model; 
                    PriorModel.Rows.Add(t);
                    PriorModel.Rows.Remove(t);
                    break;
                case KomBank2.Mmb: 
                    MmbModel = model; 
                    MmbModel.Rows.Add(t);
                    MmbModel.Rows.Remove(t);
                    break;
                case KomBank2.Bps: 
                    BpsModel = model; 
                    BpsModel.Rows.Add(t);
                    BpsModel.Rows.Remove(t);
                    break;
                case KomBank2.Bgpb: 
                    BgpbModel = model;
                    BgpbModel.Rows.Add(t);
                    BgpbModel.Rows.Remove(t);
                    break;
            }

            _countOfReady++;
            if (_countOfReady == 5)
                AllBanksLoaded = true;
        }
    }
}