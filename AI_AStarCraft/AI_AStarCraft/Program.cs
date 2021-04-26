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

        public static string testJson1 = @"
        {
            ""Map"": ""###################\n###################\n###............####\n###################\n###################\n###################\n###################\n###............####\n###################\n###################\n"",
            ""Robots"": [ ""3 2 R"", ""14 7 L"" ]
        }
";

        public static string testJson2 = @"
        {
            ""Map"": ""###################\n###################\n###################\n########.U.########\n########...########\n########...########\n###################\n###################\n###################\n###################\n"",
            ""Robots"": [ ""9 5 D"" ]
        }
";


        public static string testJson3 = @"
        {
            ""Map"": ""L..#############..D\n...#############...\n...#############...\n###################\n###################\n###################\n###################\n...#############...\n...#############...\nU..#############..R\n"",
            ""Robots"": [ ""2 2 L"", ""16 2 D"", ""2 7 U"", ""16 7 R"" ]
        }
";
        static void Main(string[] args)
        {
            var map = Map.ParseMap(testJson3);
            var problem = new AStarCraftProblem(map);
            var solution = new AStarCraftSolver().GetSolutions(problem);            
        }
    }
}