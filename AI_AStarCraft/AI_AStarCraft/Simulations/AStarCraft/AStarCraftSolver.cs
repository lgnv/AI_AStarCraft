using System.Collections.Generic;
using System.Numerics;
using AI_AStarCraft.Algorithms;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class AStarCraftSolver: ISolver<AStarCraftProblem, AStarCraftSolution>
    {
        public IEnumerable<AStarCraftSolution> GetSolutions(AStarCraftProblem problem)
        {
            yield return new AStarCraftSolution(new []{ new Arrow(new Vector2(1, 8), Direction.Down) });
        }
    }
}