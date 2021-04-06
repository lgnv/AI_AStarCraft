using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AI_AStarCraft.Visualization
{
    public class Visualization
    {
        public static void Draw(string testJson)
        {
            var jsonPath = File.ReadAllText("1.json");
            var path = Directory.GetParent("../../../vis.json");
            var content = new StringBuilder();
            content.Append("let data = '");
            content.Append(jsonPath);
            content.Append("'\nlet start = ");
            content.Append(Regex.Replace(testJson, @"\s+", " "));
            File.WriteAllText(path + "/Visualization/vis.js", content.ToString());
            Console.WriteLine("Open it in browser:\n" + "file:///" + path + "/Visualization/index.html");
        }
    }
}