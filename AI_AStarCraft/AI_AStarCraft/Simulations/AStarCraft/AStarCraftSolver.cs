using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using AI_AStarCraft.Algorithms;

namespace AI_AStarCraft.Simulations.AStarCraft
{
    public class AStarCraftSolver: ISolver<AStarCraftProblem, AStarCraftSolution>
    {
        private Dictionary<Automaton2000, HashSet<(Vector2, Direction)>> MovementHistory { get; set; }
        private Map Map { get; set; }

        static Stopwatch timer;

        public double maxScore;
        public double currentScore;

        static Arrow[] maxArrows;

        static int triesNumber;

        public HashSet<HashSet<Arrow>> ExistingVariantsHash { get; set; }
        //static List<Arrow> arrows = new List<Arrow>();

        
        public IEnumerable<AStarCraftSolution> GetSolutions(AStarCraftProblem problem)
        {
            triesNumber = 0;
            ExistingVariantsHash = new HashSet<HashSet<Arrow>>();
            maxScore = 0;
            Map = problem.Map;
            MovementHistory = Map
                .Automaton2000s
                .ToDictionary(a => a, a => new HashSet<(Vector2, Direction)>());

            timer = Stopwatch.StartNew();
            //yield return new AStarCraftSolution(new []{ new Arrow(new Vector2(1, 8), Direction.Down) });
            try
            {
                FindBestSolution(problem, new HashSet<Arrow>());
                Console.WriteLine(maxScore);
                var solution = new AStarCraftSolution(maxArrows, maxScore);
                return new List<AStarCraftSolution>() { solution };
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("timeout ");
                return null;
            }

        }


        //public IEnumerable<AStarCraftSolution> FindBestSolution(AStarCraftProblem problem, List<Arrow> arrows)
        //public void FindBestSolution(AStarCraftProblem problem, List<Arrow> arrows)
        //public IEnumerable<AStarCraftSolution> FindBestSolution(AStarCraftProblem problem, List<Arrow> arrows)
        public void FindBestSolution(AStarCraftProblem problem, HashSet<Arrow> arrows)
        {
            var arrowsSetsList = new List<HashSet<Arrow>>();
            var broken = new List<Automaton2000>();
            var brokenCount = 0;
            //if (timer.ElapsedMilliseconds > 990) throw new TimeoutException();
            //if (timer.ElapsedMilliseconds > 3000) throw new TimeoutException();
            for (var i = 0; i < Map.Automaton2000s.Length; i++)
            {
                var automaton2000 = Map.Automaton2000s[i];
                automaton2000.Reset();
            }
            foreach (var key in Map.Cells.Keys)
            {
                Map.Cells[key].Direction = null;
            }
            foreach (var arrow in arrows)
            {
                Map.Cells[arrow.Location].Direction = arrow.Direction;
            }


            MovementHistory.Clear();
            MovementHistory = Map
                .Automaton2000s
                .ToDictionary(a => a, a => new HashSet<(Vector2, Direction)>());

            currentScore = 0;
            while (!IsFinished())
            {
                Tick();
                
                broken = Map.Automaton2000s.Where(x => x.Broken).ToList();
                if (broken.Count > brokenCount)
                {
                    var newarrows = Analyse(broken);
                    /*
                    Console.WriteLine("arrows");
                    foreach(var arrow in arrows)
                    {
                        Console.WriteLine("arrow point" + arrow.Location + " " + arrow.Direction);
                    }
                    */
                    arrowsSetsList.Add(newarrows);
                    brokenCount++;
                }
                
            }
            if (currentScore > maxScore)
            {
                maxScore = currentScore;
                maxArrows = arrows.ToArray();
                Console.WriteLine(maxScore);
                Console.WriteLine("triesNumber " + triesNumber);
                for (var i = 0; i < Map.Automaton2000s.Length; i++)
                {
                    var automaton2000 = Map.Automaton2000s[i];
                    Console.WriteLine("start " + automaton2000.OriginalLocation.ToString());
                    Console.WriteLine("end " + automaton2000.Location.ToString());
                    Console.WriteLine("pathLength " + MovementHistory[automaton2000].Count);
                }
                foreach (var arrow in arrows)
                {
                    Console.WriteLine("arrow point" + arrow.Location + " " + arrow.Direction);
                }
            }
            //var longestRobot = MovementHistory.FirstOrDefault(x => x.Value == MovementHistory.Values.Max()).Key;
            //Console.WriteLine("currentScore " + currentScore.ToString());
            foreach(var arrowset in arrowsSetsList)
            {
                //if (!ExistingVariantsHash.Contains(arrowset))
                //{
                //    ExistingVariantsHash.Add(arrowset);
                    triesNumber++;
                    
                    FindBestSolution(problem, arrowset);
                   
                //}
            }
            //Console.WriteLine(maxScore);
            //var solution = new AStarCraftSolution(maxArrows, maxScore);
            //return new List<AStarCraftSolution>() { solution};
        }

        public bool IsFinished()
            => Map.Automaton2000s.All(a => a.Broken);

        public void Tick()
        {
            if (IsFinished())
                return;
            //Console.WriteLine("new Round");
            for (var i = 0; i < Map.Automaton2000s.Length; i++)
            {
                var automaton2000 = Map.Automaton2000s[i];
                //Console.WriteLine("automaton2000 old");
                //Console.WriteLine(automaton2000.Location.X.ToString() + " " + automaton2000.Location.Y.ToString());
                //Console.WriteLine(automaton2000.Direction);

                if (automaton2000.Broken)
                    continue;
                var newLocation = automaton2000.Move();
                //Console.WriteLine("new location " + newLocation.ToString());
                var oldDirection = automaton2000.Direction;
                if (Map.Cells.TryGetValue(newLocation, out var currentCell))
                    automaton2000.Interact(currentCell);
                if (!automaton2000.Broken)
                {
                    if (MovementHistory[automaton2000].Contains((newLocation, automaton2000.Direction)))
                    {
                        automaton2000.Break();
                        automaton2000.Direction = oldDirection;
                        continue;
                    }
                    automaton2000.Location = newLocation;
                    MovementHistory[automaton2000].Add((newLocation, automaton2000.Direction));
                    /*
                    try
                    {
                        MovementHistory[automaton2000].Add((newLocation, automaton2000.Direction));
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("exception " + ex.Message);
                    }
                    */
                    currentScore++;
                    //Console.WriteLine("automaton2000 new");
                    //Console.WriteLine(automaton2000.Location.X.ToString() + " " + automaton2000.Location.Y.ToString());
                    //Console.WriteLine(automaton2000.Direction);
                }

                //currentScore++;
                //if (MovementHistory[automaton2000].Contains((newLocation, automaton2000.Direction)))
                //    automaton2000.Break();
                //MovementHistory[automaton2000].Add((newLocation, automaton2000.Direction));
                //if (Map.Cells.TryGetValue(newLocation, out var currentCell))
                //    automaton2000.Interact(currentCell);
            }
        }

        public HashSet<Arrow> Analyse(List<Automaton2000> brokenList)
        {
            var arrows = new HashSet<Arrow>();
            foreach (var broken in brokenList)
            {
                //Console.WriteLine("variants for " + broken.Location.ToString());
                broken.Broken = false;
                foreach (var direct in Enum.GetValues(typeof(Direction)))
                {
                    broken.Direction = (Direction)direct;
                    //Console.WriteLine("new direction for " + broken.Location.ToString() + " " + broken.Direction.ToString());
                    var newLocation = broken.Move();
                    if (Map.Cells.TryGetValue(newLocation, out var currentCell))
                    {
                        if (currentCell.IsPlatform)
                        {
                            if (currentCell.Direction == null)
                            {
                                arrows.Add(new Arrow(broken.Location, (Direction)direct));
                                broken.Broken = true;
                                break;
                            }
                            else if (!MovementHistory[broken].Contains((newLocation, currentCell.Direction.Value)))
                            {
                                arrows.Add(new Arrow(broken.Location, (Direction)direct));
                                broken.Broken = true;
                                break;
                            }
                        }
                    }
                }
            }
            return arrows;
        }
    }
}