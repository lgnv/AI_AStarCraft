using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using AI_AStarCraft.Helpers;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class Map
    {
        public Automaton2000[] Automaton2000s { get; }
        public Dictionary<Vector2, Cell> Cells { get; }
        public int Width { get; }
        public int Height { get; }

        public Map(IEnumerable<Automaton2000> automaton2000S, Dictionary<Vector2, Cell> cells)
        {
            Automaton2000s = automaton2000S.ToArray();
            Cells = cells;
            Width = (int)Cells.Keys.Max(location => location.X);
            Height = (int)Cells.Keys.Max(location => location.Y);
        }
        
        
        public static Dictionary<Vector2, Cell> ParseCells(string level)
        {
            var lines = level.Split('\n');
            var map = new Dictionary<Vector2, Cell>();
            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    var current = lines[y][x];
                    var location = new Vector2(x, y);
                    map[location] = Cell.Create(location, current);
                }
            }

            return map;
        }

        public static Map ParseMap(string mapString, List<string> robotsStrings)
        {
            var map = ParseCells(mapString);
            var robots = robotsStrings.Select(Automaton2000.Create);
            return new Map(robots, map);
        }

        public string ToJson()
        {
            return $@"[{Cells.Select(c => c.Value.ToString()).StrJoin(",")}]";
        }
    }
}