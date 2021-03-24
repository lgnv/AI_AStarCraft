using System.Collections.Generic;

namespace AI_AStarCraft.Algorithms
{
    public interface ISolver<in TProblem, out TSolution> where TSolution : ISolution
    {
        IEnumerable<TSolution> GetSolutions(TProblem problem);
    }
}