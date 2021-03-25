using System.Text;

namespace AI_AStarCraft.Helpers
{
    public class Logger
    {
        private StringBuilder logs;

        public Logger()
        {
            this.logs = new StringBuilder("{\"data\": {");
        }

        public void Log(string data, bool withSeparator = true)
        {
            logs.Append($"{data}");
            if (withSeparator)
                logs.Append(",");
        }

        //todo lgnv: это бред, потом уберу
        public void RemoveLastChar()
        {
            logs.Remove(logs.Length - 1, 1);
        }

        public string Build()
        {
            logs.Remove(logs.Length - 1, 1);
            logs.Append("}}");
            return logs.ToString();
        }
    }
}