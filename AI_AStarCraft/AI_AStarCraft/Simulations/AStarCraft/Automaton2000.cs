using System.Numerics;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class Automaton2000: GameObject
    {
        public Vector2 Location { get; private set; }
        public Direction Direction { get; private set; }
        public bool Broken { get; private set; }
        
        public Automaton2000(Vector2 location, Direction direction, bool isBroken = false)
        {
            Location = location;
            Direction = direction;
            Broken = isBroken;
        }

        // todo lgnv: нужно знать размеры поля, чтобы брать модуль от поля(поле зациклено)
        public Vector2 Move()
        {
            if (Broken)
                return Location;
            Location = Vector2.Add(Location, Direction.GetShift());
            return Location;
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
                        Break();
                    if (cell.Direction.HasValue)
                        Direction = cell.Direction.Value;
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