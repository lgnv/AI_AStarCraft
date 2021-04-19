using System;
using System.Numerics;

namespace AI_AStarCraft.Simulations
{
    public enum Direction
    {
        Up = 'U',
        Left = 'L',
        Right = 'R',
        Down = 'D'
    }

    public static class DirectionExtensions
    {
        public static Vector2 GetShift(this Direction direction)
        {
            return direction switch
            {
                Direction.Left => new Vector2(-1, 0),
                Direction.Right => new Vector2(1, 0),
                Direction.Down => new Vector2(0, -1),
                Direction.Up => new Vector2(0, 1),
                _ => throw new ArgumentException()
            };
        }

        public static Direction Parse(char symbol)
        {
            return symbol switch
            {
                'D' => Direction.Down,
                'U' => Direction.Up,
                'L' => Direction.Left,
                'R' => Direction.Right,
                _ => throw new ArgumentException()
            };
        }
    }
}