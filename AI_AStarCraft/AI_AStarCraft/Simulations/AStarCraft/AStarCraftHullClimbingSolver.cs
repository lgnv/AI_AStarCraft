using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using AI_AStarCraft.Algorithms;

namespace AI_AStarCraft.Simulations.AStarCraft
{

    public class AStarCraftMutator
    {
        private readonly Random random;

        public AStarCraftMutator()
        {            
        }

        public AStarCraftMutation Mutate(AStarCraftProblem problem, AStarCraftSolution parentSolution)
        {
            return new AStarCraftMutation(problem, parentSolution);
        }
    }

    public class AStarCraftMutation
    {
        public double Score { get; set; }

        AStarCraftProblem Problem { get; set; }

        AStarCraftSolution ParentSolution { get; set; }

        public AStarCraftMutation(AStarCraftProblem problem, AStarCraftSolution parentSolution)
        {
            Problem = problem;
            ParentSolution = parentSolution;
            GetResult();
        }

        public AStarCraftSolution GetResult()
        {
            double max = 0;
            IEnumerable<HashSet<Arrow>> maxIen = null;
            Arrow[] arrows = null;
            //Console.WriteLine("length" + new List<HashSet<Arrow>>(ParentSolution.NextArrows).Count);
            foreach (var newHash in ParentSolution.NextArrows)
            {
                var solutions = new AStarCraftSolver().GetSolutions(Problem, newHash);
                foreach (var solution in solutions)
                {
                    //Console.WriteLine("there" + solution.Score);
                    if (solution.Score > max)
                    {
                        Score = solution.Score;
                        max = solution.Score;
                        maxIen = solution.NextArrows;
                        arrows = solution.Arrows;
                    }
                }
            }
            Console.WriteLine("thereee " + max);
            return new AStarCraftSolution(arrows, max, maxIen);
        }

    }

        public class AStarCraftHullClimbingSolver
    {
        private readonly ISolver<AStarCraftProblem, AStarCraftSolution> baseSolver;
        protected readonly AStarCraftMutator mutator;
        private readonly bool stopOnRepeatedMutation;
        private AStarCraftMutation firstMutation;
        private int mutationsCount;
        private int improvementsCount;

        public AStarCraftHullClimbingSolver(ISolver<AStarCraftProblem, AStarCraftSolution> baseSolver, AStarCraftMutator mutator,
            bool stopOnRepeatedMutation = false)
        {
            this.baseSolver = baseSolver;
            this.mutator = mutator;
            this.stopOnRepeatedMutation = stopOnRepeatedMutation;
        }

        protected bool ShouldContinue { get; set; }

        public IEnumerable<AStarCraftSolution> GetSolutions(AStarCraftProblem problem)
        {
            mutationsCount = 0;
            improvementsCount = 0;
            ShouldContinue = true;
            var steps = new List<AStarCraftSolution>();
            steps.Add(baseSolver.GetSolutions(problem, new HashSet<Arrow>()).OrderBy(x => x.Score).Last());
            Console.WriteLine(steps[0].Score);
            var timer = Stopwatch.StartNew();
            try
            {
                while (true)
                {
                    if (timer.ElapsedMilliseconds > 1000) throw new TimeoutException();
                    var improvements = Improve(problem, steps.Last());
                    mutationsCount++;
                    foreach (var solution in improvements)
                    {
                        improvementsCount++;
                        steps.Add(solution);
                    }

                    if (!ShouldContinue) break;
                }
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("timeout ");                
            }

            return steps;
        }

        protected IEnumerable<AStarCraftSolution> Improve(AStarCraftProblem problem, AStarCraftSolution bestSolution)
        {
            var mutation = mutator.Mutate(problem, bestSolution);
            if (firstMutation == null)
                firstMutation = mutation;
            else if (stopOnRepeatedMutation && mutation.Equals(firstMutation))
                ShouldContinue = false;
            if (mutation.Score > bestSolution.Score)
            {
                bestSolution = mutation.GetResult();
                firstMutation = null;
                yield return bestSolution;
            }
        }
    }
}
