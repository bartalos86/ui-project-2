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

        public bool IsInside(List<Step> steps)
        {
            int x = StartPosition.X;
            int y = StartPosition.Y;
            foreach(var step in steps)
            {
                switch (step)
                {
                    case Step.H:
                        y++;
                        break;
                    case Step.D:
                        y--;
                        break;
                    case Step.P:
                        x++;
                        break;
                    case Step.L:
                        x--;
                        break;
                }

                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return (false);

            }
            return true;
        }

        public (Position, List<Position>) PerfromsSteps(List<Step> steps)
        {
            int x = StartPosition.X;
            int y = StartPosition.Y;
            List<Position> goldCollected = new List<Position>();
            foreach (var step in steps)
            {
                switch (step)
                {
                    case Step.H:
                        y++;
                        break;
                    case Step.D:
                        y--;
                        break;
                    case Step.P:
                        x++;
                        break;
                    case Step.L:
                        x--;
                        break;
                }

                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return (new Position(x, y), goldCollected);

                if (Values[y][x] == 1)
                    goldCollected.Add(new Position(x, y));

            }
            return (new Position(x, y), goldCollected);
        }

        public double GetDistance(Position positionA, Position positionB)
        {
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
    }
}
