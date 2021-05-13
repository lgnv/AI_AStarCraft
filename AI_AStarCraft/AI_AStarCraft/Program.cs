using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var input = "...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#...................#.#.#.#.#.#.#.#.#.#|1 0 R|17 0 D|1 8 U|17 8 L";
            var m = new ProblemParcer();
            var map = m.GetMapFromInput(input);
            var problem = new AStarCraftProblem(map);
            var solutions = new AStarCraftSolver().GetSolutions(problem, new HashSet<Arrow>());
            foreach(var solution in solutions)
            {
                Console.WriteLine("score " + solution.Score + "new " + new List<HashSet<Arrow>>(solution.NextArrows).Count);
            }
            /*
            var mutator = new AStarCraftMutator();
            var mutation = mutator.Mutate(problem, solutions.OrderBy(x => x.Score).Last());
            var newSolution = mutation.GetResult();
            Console.WriteLine("new score " + newSolution.Score);
            */
            var solver = new AStarCraftHullClimbingSolver(new AStarCraftSolver(), new AStarCraftMutator(), false);
            var newsolutions = solver.GetSolutions(new AStarCraftProblem(map));
            foreach (var solution in newsolutions)
            {
                Console.WriteLine("all new score " + solution.Score + "new " + new List<HashSet<Arrow>>(solution.NextArrows).Count);
            }
        }
    }

        public class ProblemParcer
        {

            public Map GetMapFromInput(string input)
            {                
                var newinL = new List<String>();
                for (var i = 0; i < 190; i += 19)
                {
                    var part = input.Substring(i, 19);
                    newinL.Add(part);
                }
                var newin = string.Join('\n', newinL);       
                var robotsString = input.Substring(190, input.Length - 190);
                var robotsList = robotsString.Split('|', StringSplitOptions.RemoveEmptyEntries);           
                var map = Map.ParseMap(newin, new List<String>(robotsList));
            
                return map;
            }
        }

}