using System.Collections.Generic;

namespace Extractors
{
    public class RootObject
    {
        public string Message { get; set; }
        public string Locale { get; set; }
        public object FullList { get; set; }
        public List<FullListAjax> FullListAjax { get; set; }
    }
}