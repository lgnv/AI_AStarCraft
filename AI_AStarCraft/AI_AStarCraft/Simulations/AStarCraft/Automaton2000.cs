using System;
using System.Numerics;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class Automaton2000: GameObject
    {
        public Vector2 Location { get; set; }
        public Direction Direction { get; set; }
        public bool Broken { get; set; }
        public bool Analized { get; set; }


        public Vector2 OriginalLocation { get; private set; }
        public Direction OriginalDirection { get; private set; }

        public Automaton2000(Vector2 location, Direction direction, bool isBroken = false)
        {
            Location = location;
            Direction = direction;
            Broken = isBroken;
            Analized = false;
            OriginalLocation = new Vector2(location.X, location.Y);
            if (direction == Direction.Up)
                OriginalDirection = Direction.Up;
            if (direction == Direction.Down)
                OriginalDirection = Direction.Down;
            if (direction == Direction.Right)
                OriginalDirection = Direction.Right;
            if (direction == Direction.Left)
                OriginalDirection = Direction.Left;

        }

        // todo lgnv: нужно знать размеры поля, чтобы брать модуль от поля(поле зациклено)
        public Vector2 Move()
        {
            if (Broken)
                return Location;            
            var newloc = Vector2.Add(Location, Direction.GetShift());
            if (newloc.X < 0)
                newloc.X = 19 + newloc.X;
            if (newloc.X > 18)
                newloc.X = 19 - newloc.X;
            if (newloc.Y < 0)
                newloc.Y = 10 + newloc.Y;
            if (newloc.Y > 9)
                newloc.Y = 10 - newloc.Y;
            //Location = Vector2.Add(Location, Direction.GetShift());
            return newloc;
        }

        public void Reset()
        {
            Location = OriginalLocation;
            Direction = OriginalDirection;
            Broken = false;
            Analized = false;
        }

        public void Break()
            => Broken = true;

        public override void Interact(GameObject other)
        {
            if (Broken)
                return;
            switch (other)
            {
                case Cell cell:
                    if (!cell.IsPlatform)
                    {
                        Break();
                    }
                    if (cell.Direction.HasValue)
                    {
                        Direction = cell.Direction.Value;
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return $@"{{""id"":{Id},""x"":{Location.X},""y"":{Location.Y},""direction"":""{Direction}""}}";
        }

        // Принимает строку вида X Y Direction, где X, Y -- числа, Direction -- {L, R, U, D}
        public static Automaton2000 Create(string str)
        {
            var args = str.Split();
            var location = new Vector2(int.Parse(args[0]), int.Parse(args[1]));
            var direction = DirectionExtensions.Parse(args[2][0]);
            return new Automaton2000(location, direction);
        }
    }
}