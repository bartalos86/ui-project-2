using System.Text;

namespace genetic_algorithm
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            AICollection swarm = new AICollection(0.05, SelectionStyle.ROULETTE, CrossoverStyle.GENE_CROSSOVER);
            string file = args.Length > 0 ? args[0] : "testMap.txt";

            Map startingMap = ReadMap(file);
            startingMap.PrintMap();
            swarm.SetupMap(startingMap);
            swarm.InitializeAIs(200);

            while (!swarm.SolutionFound)
            {
                swarm.DoGeneration();
            }

            Console.WriteLine("do you want to continue? (c/n)");
            if (swarm.SolutionFound && Console.ReadLine() == "c")
            {
                while (true)
                {
                    swarm.DoGeneration();
                }
            }


        }

        public static Map ReadMap(string filePath)
        {
            Map map = null;

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                string line = "";
                while((line = reader.ReadLine()) != null)
                {
                    var positions = line.Split(";");

                    var type = positions[0];
                    int x = int.Parse(positions[1]);
                    int y = int.Parse(positions[2]);

                    if (type == "size")
                    {
                       map = new Map(x, y);
                    }
                    else if (type == "start") 
                    {
                        map?.AddStart(x, y);
                    }
                    else if(type == "gold")
                    {
                        map?.AddGold(x, y);
                    }

                   
                }
            }

            return map;
        }

        
    }


}
