using System.Collections.Generic;

namespace Extractors
{
    public class FullListAjax
    {
        public string CurrencyType { get; set; }
        public int ChannelId { get; set; }
        public List<ViewVOitem> ViewVoList { get; set; }
    }
}