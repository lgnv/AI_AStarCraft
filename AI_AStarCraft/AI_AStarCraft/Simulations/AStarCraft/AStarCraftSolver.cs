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

        public Stack<HashSet<Arrow>> CurrentArrowsLists;

        
        public IEnumerable<AStarCraftSolution> GetSolutions(AStarCraftProblem problem, HashSet<Arrow> startArrows)
        {
            CurrentArrowsLists = new Stack<HashSet<Arrow>>();
            triesNumber = 0;
            ExistingVariantsHash = new HashSet<HashSet<Arrow>>();
            maxScore = 0;
            Map = problem.Map;
            MovementHistory = Map
                .Automaton2000s
                .ToDictionary(a => a, a => new HashSet<(Vector2, Direction)>());            
            timer = Stopwatch.StartNew();         
           
            AnalyseStart(Map.Automaton2000s.ToList());
            //FindBestSolution(problem, new HashSet<Arrow>(), false);
            FindBestSolution(problem, startArrows, false);
            while (true)
            {
                if (CurrentArrowsLists.Count > 0)
                {
                    var currentArrows = CurrentArrowsLists.Pop();
                    var nextArrows = GetNewArrows(startArrows);
                    //ResetGame(new HashSet<Arrow>());
                    yield return new AStarCraftSolution(currentArrows.ToArray(), currentScore, nextArrows);
                    FindBestSolution(problem, currentArrows, false);                    
                }
                else
                {
                    var nextArrows = GetNewArrows(startArrows);
                    //ResetGame(new HashSet<Arrow>());
                    yield return new AStarCraftSolution(new Arrow[0], currentScore, nextArrows);
                    break;
                }
            }                                                                                                                  
        }

        public void ResetGame(HashSet<Arrow> arrows)
        {
            for (var i = 0; i < Map.Automaton2000s.Length; i++)
            {
                var automaton2000 = Map.Automaton2000s[i];
                automaton2000.Reset();
            }
            foreach (var key in Map.Cells.Keys)
            {
                if (!Map.Cells[key].IsOriginallyArrow)
                    Map.Cells[key].Direction = null;
                Map.Cells[key].IsCrossed = 0;
            }
            foreach (var arrow in arrows)
            {
                Map.Cells[arrow.Location].Direction = arrow.Direction;
                for (var i = 0; i < Map.Automaton2000s.Length; i++)
                {
                    var automaton2000 = Map.Automaton2000s[i];
                    if (automaton2000.Location == arrow.Location)
                    {
                        automaton2000.Direction = arrow.Direction;
                    }
                }
            }


            for (var i = 0; i < Map.Automaton2000s.Length; i++)
            {
                var automaton2000 = Map.Automaton2000s[i];

                MovementHistory[automaton2000].Add((automaton2000.Location, automaton2000.Direction));
                Map.Cells[automaton2000.Location].IsCrossed = 1;
            }

            MovementHistory.Clear();
            MovementHistory = Map
                .Automaton2000s
                .ToDictionary(a => a, a => new HashSet<(Vector2, Direction)>());
        }
        
        public void FindBestSolution(AStarCraftProblem problem, HashSet<Arrow> arrows, bool isFinal)
        {            
            var arrowsSetsList = new List<HashSet<Arrow>>();
            var broken = new List<Automaton2000>();
            var brokenCount = 0;
            if (timer.ElapsedMilliseconds > 500) throw new TimeoutException();

            ResetGame(arrows);            

            currentScore = 0;
            while (!IsFinished())
            {
                Tick();
                
                broken = Map.Automaton2000s.Where(x => x.Broken).ToList();
                if (!isFinal)
                {
                    if (broken.Count > brokenCount)
                    {
                        var newarrows = Analyse(broken);
                        if (newarrows.Count > 0)
                        {
                            foreach (var newarrow in newarrows)
                            {
                                var newList = new HashSet<Arrow>(arrows);
                                newList.Add(newarrow);
                                CurrentArrowsLists.Push(newList);
                            }
                        }
                        var oldBroken = brokenCount;
                        brokenCount = broken.Count - oldBroken;
                    }
                }
                
            }            
            if (currentScore > maxScore)
            {
                maxScore = currentScore;                
                maxArrows = arrows.ToArray();
                //Console.WriteLine(maxScore);
            }            
        }

        public void WriteCurrentArrows(HashSet<Arrow> arrows)
        {
            Console.WriteLine("current arrows:");
            foreach(var arrow in arrows)
            {
                Console.Write(arrow.Location + " " + arrow.Direction + " ");
            }
            Console.WriteLine(" ");
        }

        public bool IsFinished()
            => Map.Automaton2000s.All(a => a.Broken);

        public void Tick()
        {
            if (IsFinished())
                return;           
            for (var i = 0; i < Map.Automaton2000s.Length; i++)
            {
                var automaton2000 = Map.Automaton2000s[i];                

                if (automaton2000.Broken)
                    continue;
                var newLocation = automaton2000.Move();                
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
                    Map.Cells[newLocation].IsCrossed++;
                    currentScore++;                    
                }                
            }
        }

        public HashSet<Arrow> Analyse(List<Automaton2000> brokenList)
        {
            var arrows = new HashSet<Arrow>();            
            foreach (var broken in brokenList)
            {
                if (broken.Analized)
                    continue;
                broken.Broken = false;                
                if (Map.Cells[broken.Location].IsCrossed > 1 || Map.Cells[broken.Location].IsOriginallyArrow)
                {
                    broken.Broken = true;
                    broken.Analized = true;
                    continue;
                }
                
                foreach (var direct in Enum.GetValues(typeof(Direction)))
                {
                    broken.Direction = (Direction)direct;                    
                    var newLocation = broken.Move();
                    if (Map.Cells.TryGetValue(newLocation, out var currentCell))
                    {
                        if (currentCell.IsPlatform)
                        {
                            if (currentCell.Direction == null)
                            {
                                arrows.Add(new Arrow(broken.Location, (Direction)direct));
                            }
                            else if (!MovementHistory[broken].Contains((newLocation, currentCell.Direction.Value)))
                            {
                                arrows.Add(new Arrow(broken.Location, (Direction)direct));                                
                            }
                        }
                    }
                }
            broken.Broken = true;
            broken.Analized = true;
            }            
            return arrows;
        }

        
        
        public void AnalyseStart(List<Automaton2000> robots)
        {
            ResetGame(new HashSet<Arrow>());
            var arrows = new HashSet<Arrow>();
            foreach (var broken in robots)
            {                
                if (Map.Cells[broken.Location].IsOriginallyArrow)
                {
                    continue;
                }

                foreach (var direct in Enum.GetValues(typeof(Direction)))
                {
                    broken.Direction = (Direction)direct;
                    var newLocation = broken.Move();
                    if (Map.Cells.TryGetValue(newLocation, out var currentCell))
                    {
                        if (currentCell.IsPlatform)
                        {
                            if (currentCell.Direction == null)
                            {
                                arrows.Add(new Arrow(broken.Location, (Direction)direct));
                            }
                           
                        }
                    }
                }
                //broken.Broken = true;
                //broken.Analized = true;
            }
            if (arrows.Count > 0)
            {
                var newList = new HashSet<Arrow>();
                foreach (var newarrow in arrows)
                {                    
                    newList.Add(newarrow);                    
                }
                CurrentArrowsLists.Push(newList);
            }
        }

        /*
        public void AnalyseCells(AStarCraftProblem problem)
        {
            CurrentArrowsLists = new Stack<HashSet<Arrow>>();
            var brokenList = Map.Automaton2000s.Where(x => x.Broken).ToList();
            var lc = new List<Arrow>();
            var newStack = new Stack<(Vector2, Direction)>();
            var last = brokenList.First();            
            var count = 0;
            foreach (var move in MovementHistory[last])
            {
                if (count > 10)
                    break;
                newStack.Push(move);
                count++;
            }
            while (newStack.Count != 0)
            {
                var old = newStack.Pop();
                foreach (var direct in Enum.GetValues(typeof(Direction)))
                {
                    if (Map.Cells[old.Item1].Direction != null)
                        continue;
                    if ((Direction)direct != old.Item2)
                    {
                        var newlock = GetNear(old.Item1, (Direction)direct);
                        if (Map.Cells[newlock].IsPlatform)
                        {                            
                            var newList = new HashSet<Arrow>(maxArrows);
                            newList.Add(new Arrow(old.Item1, (Direction)direct));
                            CurrentArrowsLists.Push(newList);                            
                        }
                    }
                }
            }            
            var oldMaxScore = maxScore;
            var brokenId = 0;
            while (true)
            {
                if (CurrentArrowsLists.Count > 0)
                    FindBestSolution(problem, CurrentArrowsLists.Pop(), false);
                else
                {
                    if (maxScore > oldMaxScore)
                    {
                        oldMaxScore = maxScore;                        
                        FindBestSolution(problem, new HashSet<Arrow>(maxArrows), true);
                    }
                    else
                    {
                        while (CurrentArrowsLists.Count == 0)
                        {
                            brokenId++;
                            if (brokenId >= brokenList.Count)
                                return;
                            GetNewArrows(brokenId);
                        }
                    }
                }
            }
        }
         */
        public IEnumerable<HashSet<Arrow>> GetNewArrows(HashSet<Arrow> startArrows)
        {
            var brokenList = Map.Automaton2000s.Where(x => x.Broken).ToList();
            var lc = new List<Arrow>();
            var newStack = new Stack<(Vector2, Direction)>();
                        
            foreach (var broken in brokenList)
            {
                var count = 0;
                foreach (var move in MovementHistory[broken])
                {
                    if (count > 20)
                        break;
                    newStack.Push(move);
                    count++;
                }
                while (newStack.Count != 0)
                {
                    var old = newStack.Pop();
                    foreach (var direct in Enum.GetValues(typeof(Direction)))
                    {
                        if (Map.Cells[old.Item1].Direction != null)
                            continue;
                        if ((Direction)direct != old.Item2)
                        {
                            var newlock = GetNear(old.Item1, (Direction)direct);
                            if (Map.Cells[newlock].IsPlatform)
                            {
                                var newHash = new HashSet<Arrow>(startArrows);
                                newHash.Add(new Arrow(old.Item1, (Direction)direct));
                                yield return newHash;
                            }
                        }
                    }
                }
            }
        }

        public Vector2 GetNear(Vector2 place, Direction direction)
        {
            var newloc = Vector2.Add(place, direction.GetShift());
            if (newloc.X < 0)
                newloc.X = 19 + newloc.X;
            if (newloc.X > 18)
                newloc.X = 19 - newloc.X;
            if (newloc.Y < 0)
                newloc.Y = 10 + newloc.Y;
            if (newloc.Y > 9)
                newloc.Y = 10 - newloc.Y;            
            return newloc;
        }
       

    }
}