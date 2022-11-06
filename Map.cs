using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace genetic_algorithm
{
    public class Map
    {
        public int[][] Values { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Position StartPosition { get; private set; }
        public List<Position> GoldPositions { get; set; }

        private bool startExists = false;

        public Map(int width = 7, int height = 7)
        {
            Width = width;
            Height = height;
            GoldPositions = new List<Position>();
            InitializeValues();
        }

        private void InitializeValues()
        {
            Values = new int[Height][];
            for (int i = 0; i < Height; i++)
            {
                Values[i] = new int[Width];
                for (int j = 0; j < Width; j++)
                {
                    Values[i][j] = 0;
                }
            }
               
        }

        public (bool, int) IsInside(List<Step> steps)
        {
            int x = StartPosition.X;
            int y = StartPosition.Y;
            int stepNum = 0;
            foreach(var step in steps)
            {
                switch (step)
                {
                    case Step.H:
                        y--;
                        break;
                    case Step.D:
                        y++;
                        break;
                    case Step.P:
                        x++;
                        break;
                    case Step.L:
                        x--;
                        break;
                }

                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return (false, stepNum);

                stepNum++;

            }
            return (true,-1);
        }

        public (Position, List<Position>) PerfromsSteps(List<Step> steps)
        {
            int x = StartPosition.X;
            int y = StartPosition.Y;
            List<Position> goldCollected = new List<Position>();
           
            foreach (var step in steps)
            {
                Position prevPosition = new Position(x, y);
                switch (step)
                {
                    case Step.H:
                        y--;
                        break;
                    case Step.D:
                        y++;
                        break;
                    case Step.P:
                        x++;
                        break;
                    case Step.L:
                        x--;
                        break;
                }

                var position = new Position(x, y);
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return (null, goldCollected);

                if (Values[y][x] == 1 && !goldCollected.ContainsByHash(position))
                {
                    goldCollected.Add(position);

                }

             

            }
            return (new Position(x, y), goldCollected);
        }

        public double GetDistance(Position positionA, Position positionB)
        {
            if (positionA == null || positionB == null)
                return Double.NaN;

            double distance = Math.Sqrt(Math.Pow(positionA.X-positionB.X,2) + Math.Pow(positionA.Y-positionB.Y,2));
            return distance;
        }

        public bool AddStart(int x, int y)
        {
            try
            {
                if (Values[y][x] != 0 || startExists)
                    return false;
                else
                {
                    Values[y][x] = 2;
                    StartPosition = new Position(x, y);
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddGold(int x, int y)
        {
            try
            {
                if (Values[y][x] == 1)
                    return false;
                else
                {
                    Values[y][x] = 1;
                    GoldPositions.Add(new Position(x, y));
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        public void PrintMap()
        {
            for (int i = 0; i < Height+2; i++)
            {
                for (int j = 0; j < Width+2; j++)
                {
                    if(i == 0 || i > Height)
                    {
                        Console.Write("--");
                        continue;
                    }

                    if(j == 0 || j > Width)
                    {
                        Console.Write("| ");
                        continue;
                    }

                    if (Values[i-1][j-1] == 2)
                    {
                        Console.Write("S ");
                    }
                    else if(Values[i - 1][j - 1] == 1)
                    {
                        Console.Write("G ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
              
        }

        public void PrintOutPath(List<Step> steps)
        {
            var map = Values.DeepClone();
            int x = StartPosition.X;
            int y = StartPosition.Y;
            foreach(var step in steps)
            {
                switch (step)
                {
                    case Step.H:
                        y--;
                        break;
                    case Step.D:
                        y++;
                        break;
                    case Step.P:
                        x++;
                        break;
                    case Step.L:
                        x--;
                        break;
                }

                if(map[y][x] < 5)
                    map[y][x] = 5;
                else
                    map[y][x] = map[y][x]+1;
            }

            for (int i = 0; i < Height + 2; i++)
            {
                for (int j = 0; j < Width + 2; j++)
                {
                    Console.BackgroundColor = ConsoleColor.Black;

                    if (i == 0 || i > Height)
                    {
                        Console.Write("--");
                        continue;
                    }

                    if (j == 0 || j > Width)
                    {
                        Console.Write("| ");
                        continue;
                    }

                    if (map[i - 1][j - 1] >= 5)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    }

                    if (map[i - 1][j - 1] >= 6)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }

                    if (map[i - 1][j - 1] >= 8)
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                    }

                    if (map[i - 1][j - 1] < 5)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }

                    if (Values[i - 1][j - 1] == 2)
                    {
                        Console.Write("S ");
                    }
                    else if (Values[i - 1][j - 1] == 1)
                    {
                        Console.Write("G ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
