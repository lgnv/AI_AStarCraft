using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using AI_AStarCraft.Algorithms;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class AStarCraftSolution: ISolution
    {
        public double Score { get; }
        public Arrow[] Arrows { get; }

        public IEnumerable<HashSet<Arrow>> NextArrows;

        public AStarCraftSolution(Arrow[] arrows, double score, IEnumerable<HashSet<Arrow>> nextArrows)
        {
            Arrows = arrows;
            Score = score;
            NextArrows = nextArrows;
        }
    }
}