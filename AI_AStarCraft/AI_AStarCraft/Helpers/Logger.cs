using System.Text;

namespace AI_AStarCraft.Helpers
{
    public class Logger
    {
        private StringBuilder logs;

        public Logger()
        {
            this.logs = new StringBuilder("{\"data\":[");
        }

        public void Log(string data)
            => logs.Append($"{data},");

        public string Build()
        {
            logs.Remove(logs.Length - 1, 1);
            logs.Append("]}");
            return logs.ToString();
        }
    }
}