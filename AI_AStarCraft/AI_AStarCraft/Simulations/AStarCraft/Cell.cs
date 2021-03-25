using System;
using System.Collections.Generic;
using System.Numerics;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class Cell: GameObject
    {
        public Vector2 Location { get; }
        public Direction? Direction { get; set; }
        public bool IsPlatform { get; }
        
        private static Dictionary<char, Func<Vector2, Cell>> createByCharacter = new Dictionary<char, Func<Vector2, Cell>>
        {
            ['#'] = location => new Cell(location, false),
            ['U'] = location => new Cell(location, true, Simulations.Direction.Up),
            ['D'] = location => new Cell(location, true, Simulations.Direction.Down),
            ['R'] = location => new Cell(location, true, Simulations.Direction.Right),
            ['L'] = location => new Cell(location, true, Simulations.Direction.Left),
            ['.'] = location => new Cell(location, true),
        };

        public Cell(Vector2 location, bool isPlatform, Direction? direction = null)
        {
            this.Location = location;
            this.IsPlatform = isPlatform;
            this.Direction = direction;
        }

        public static Cell Create(Vector2 location, char symbol)
            => createByCharacter[symbol](location);

        public static bool TryCreate(Vector2 location, char symbol, out Cell cell)
        {
            if (createByCharacter.TryGetValue(symbol, out var creator))
            {
                cell = creator(location);
                return true;
            }

            cell = default;
            return false;

        }

        public override string ToString()
        {
            var direction = Direction.HasValue
                ? @$", ""direction"": ""{Direction.Value}"""
                : "";
            return @$"{{""x"":{Location.X}, ""y"":{Location.Y}{direction} }}";
        }

        public override void Interact(GameObject other) { }
    }
}