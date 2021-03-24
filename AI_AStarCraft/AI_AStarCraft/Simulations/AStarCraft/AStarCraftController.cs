using System;
using System.Linq;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class MapDto
    {
        public string Map { get; set; }
        public string[] Robots { get; set; }
    }
    
    public class AStarCraftController
    {
        public static void Play(Map map)
        {
            var problem = new AStarCraftProblem(map);
            var solution = new AStarCraftSolver().GetSolutions(problem).First();
            
            ApplySolution(solution, map);
            
            var state = new AStarCraftState(map);
            while (!state.IsFinished())
            {
                state.Tick();
                // Чтобы понять, что работает
                Console.WriteLine(state.ToString());
                Console.WriteLine();
                //
            }
        }

        private static Map ApplySolution(AStarCraftSolution solution, Map map)
        {
            // todo lgnv: возможно переделать на то, чтобы не менять я возврашать новое
            foreach (var arrow in solution.Arrows)
                map.Cells[arrow.Location].Direction = arrow.Direction;
            return map;
        }
    }
}