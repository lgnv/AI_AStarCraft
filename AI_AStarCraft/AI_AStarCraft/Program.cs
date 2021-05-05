using System;
using System.Collections.Generic;
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
        public static string testJson16 = @"
        {
            ""Map"": ""{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n"",
            ""Robots"": [ ""2 2 L"", ""16 2 D"", ""2 7 U"", ""16 7 R"" ]
        }
";
        static void Main(string[] args)
        {
            var input = "...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#";
            var newinL = new List<String>();
            for (var i = 0; i < input.Length; i+=19)
            {
                var part = input.Substring(i, 19);
                newinL.Add(part);
            }
            var newin = string.Join("\\n", newinL);
            Console.WriteLine(string.Join("\\n", newinL));


            var testJson16 = @"
        {
            ""Map"": """ + newin + @""",
            ""Robots"": [ ""1 0 R"", ""17 0 D"", ""1 8 U"", ""17 8 L"" ]
        }
";
            //Console.WriteLine(testJson3);
            //Console.WriteLine(testJson16);
            var map = Map.ParseMap(testJson16);
            var problem = new AStarCraftProblem(map);
            var solution = new AStarCraftSolver().GetSolutions(problem);            
        }
    }
}