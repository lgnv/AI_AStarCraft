using System.Numerics;
using AI_AStarCraft.Algorithms;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class AStarCraftSolution: ISolution
    {
        public double Score { get; }
        public Arrow[] Arrows { get; }

        public AStarCraftSolution(Arrow[] arrows, double score)
        {
            Arrows = arrows;
            Score = score;
        }
    }
}