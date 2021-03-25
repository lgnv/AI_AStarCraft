using System;
using System.Linq;
using AI_AStarCraft.Helpers;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class MapDto
    {
        public string Map { get; set; }
        public string[] Robots { get; set; }
    }
    
    public class AStarCraftController
    {
        private readonly Logger Logger;

        public AStarCraftController(Logger logger)
        {
            Logger = logger;
        }

        public void Play(Map map)
        {
            var problem = new AStarCraftProblem(map);
            var solution = new AStarCraftSolver().GetSolutions(problem).First();

            ApplySolution(solution, map);
            
            var state = new AStarCraftState(map);
            while (!state.IsFinished())
            {
                state.Tick();
                Logger.Log(state.ToString());
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