using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_AStarCraft.Helpers
{
    public class Logger
    {
        private readonly Dictionary<string, List<string>> logs;

        public Logger()
        {
            logs = new Dictionary<string, List<string>>();
        }

        public void Log(string key, string data)
        {
            if (!logs.ContainsKey(key))
                logs[key] = new List<string>();
            logs[key].Add(data);
        }

        public string Build()
        {
            var stringBuilder = new StringBuilder(@"{""data"":{");
            stringBuilder.AppendJoin(",", logs.Select(kvp => Build(kvp.Key)));
            stringBuilder.Append("}}");
            return stringBuilder.ToString();
        }

        private string Build(string key)
            => @$"""{key}"":[{logs[key].StrJoin(",")}]";
    }
}