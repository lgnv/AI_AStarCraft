using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AI_AStarCraft.Helpers;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class AStarCraftState
    {
        private Dictionary<Automaton2000, HashSet<(Vector2, Direction)>> MovementHistory { get; }
        private Map Map { get; }
        private int TickCount { get; set; }

        public AStarCraftState(Map map)
        {
            Map = map;
            MovementHistory = map
                .Automaton2000s
                .ToDictionary(a => a, a => new HashSet<(Vector2, Direction)>());
        }

        public bool IsFinished()
            => Map.Automaton2000s.All(a => a.Broken);

        // todo lgnv: запоминать MovementHistory и убивать если есть уже место где мы были в текущем положении
        public void Tick()
        {
            if (IsFinished())
                return;
            for (var i = 0 ; i < Map.Automaton2000s.Length; i++)
            {
                var automaton2000 = Map.Automaton2000s[i];
                var newLocation = automaton2000.Move();
                if (Map.Cells.TryGetValue(newLocation, out var currentCell))
                    automaton2000.Interact(currentCell);
            }

            TickCount++;
        }

        public override string ToString()
        {
            return $@"{{""i"": {TickCount},""robots"":[{Map.Automaton2000s.Select(x => x.ToString()).StrJoin(",")}]}}";
        }
    }
}