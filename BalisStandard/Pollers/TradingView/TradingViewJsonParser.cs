using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BalisStandard
{
    public static class TradingViewJsonParser
    {
        public static JObject TryParse(string message)
        {
            if (!message.StartsWith("{\"m"))
                return null;

            var root = JsonConvert.DeserializeObject<TradingViewJsonRootObject>(message);
            var p = root.P[1].ToString();
            if (root.M != "qsd")
                return null;

            var branch = JsonConvert.DeserializeObject<TradingViewJsonPayloadObject>(p);
            var tikerName = branch.N;

            return JObject.Parse(branch.V.ToString());
        }

    }

    public class TradingViewJsonRootObject
    {
        public string M { get; set; }
        public List<object> P { get; set; }
    }

    public class TradingViewJsonPayloadObject
    {
        public string N { get; set; }
        public string S { get; set; }
        public object V { get; set; }

    }
}
