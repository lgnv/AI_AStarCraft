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
            /// <summary>
            ///  
            /// </summary>
            /// <param name="args"></param>
        static void Main(string[] args)
        {
            var map = Map.ParseMap(testJson);
            var controller = new AStarCraftController();
            AStarCraftController.Play(map);
        }
    }
}
