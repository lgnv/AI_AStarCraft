using System.IO;
using AI_AStarCraft.Helpers;
using AI_AStarCraft.Simulations.AStarCraft;

namespace AI_AStarCraft
{
    class Program
    {
        public static string testJson = @"
        {
            ""Map"": ""###################\n#.......###.......#\n#.#########.#######\n#.#########.#######\n#.#########.#######\n#.#########.##....#\n#.#########.#####.#\n#.#########.#####.#\n#.......###.......#\n###################"",
            ""Robots"": [ ""7 8 L"", ""7 1 L"", ""17 1 L"", ""14 5 R"" ]
        }
";
        static void Main(string[] args)
        {
            var map = Map.ParseMap(testJson);
            var logger = new Logger();
            logger.Log("map", map.ToJson());
            var controller = new AStarCraftController(logger);
            controller.Play(map);
            File.WriteAllText("1.json", logger.Build());
            Visualization.Visualization.Draw(testJson);
        }
    }
}
