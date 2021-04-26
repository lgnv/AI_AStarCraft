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

        
        public IEnumerable<AStarCraftSolution> GetSolutions(AStarCraftProblem problem)
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
            try
            {
                FindBestSolution(problem, new HashSet<Arrow>());
                while (true)
                {                    
                    if (CurrentArrowsLists.Count > 0)
                        FindBestSolution(problem, CurrentArrowsLists.Pop());
                    else
                    {
                        AnalyseStart(Map.Automaton2000s.ToList());
                        if (CurrentArrowsLists.Count > 0)
                            FindBestSolution(problem, CurrentArrowsLists.Pop());
                    }
                        break;
                }            
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("timeout ");
                Console.WriteLine(maxScore);
            }                                    
            var solution = new AStarCraftSolution(maxArrows, maxScore);
            return new List<AStarCraftSolution>() { solution };

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
        
        public void FindBestSolution(AStarCraftProblem problem, HashSet<Arrow> arrows)
        {            
            var arrowsSetsList = new List<HashSet<Arrow>>();
            var broken = new List<Automaton2000>();
            var brokenCount = 0;
            if (timer.ElapsedMilliseconds > 500) throw new TimeoutException();

            ResetGame(arrows);
            /*
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
            */


            currentScore = 0;
            while (!IsFinished())
            {
                Tick();
                
                broken = Map.Automaton2000s.Where(x => x.Broken).ToList();
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
            if (currentScore > maxScore)
            {
                maxScore = currentScore;                
                maxArrows = arrows.ToArray();
                Console.WriteLine(maxScore);
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

    }
}