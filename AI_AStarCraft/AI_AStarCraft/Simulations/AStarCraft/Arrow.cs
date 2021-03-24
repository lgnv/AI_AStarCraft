using System.Numerics;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class Arrow
    {
        public Vector2 Location { get; }
        public Direction Direction { get; }

        public Arrow(Vector2 location, Direction direction)
        {
            Location = location;
            Direction = direction;
        }
    }
}