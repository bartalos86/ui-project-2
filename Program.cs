using System.Text;

namespace genetic_algorithm
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = ShowMenu();
            AICollection swarm = new AICollection(config.Item1, config.Item2, config.Item3);
            string file = args.Length > 0 ? args[0] : "Maps/testMap.txt";

            Map startingMap = ReadMap(file);
            startingMap.PrintMap();
            swarm.SetupMap(startingMap);
            swarm.InitializeAIs(config.Item4);

            while (!swarm.SolutionFound)
            {
                swarm.DoGeneration();
            }

            Console.WriteLine("\ndo you want to continue? (c/n)");
            if (swarm.SolutionFound)
            {
                while (Console.ReadLine() == "c")
                {
                    swarm.DoGeneration();
                    Console.WriteLine("\ndo you want to continue? (c/n)");
                }

            }


        }
        //Reads in the starting configuration from the user
        public static (double, SelectionStyle, CrossoverStyle, int) ShowMenu()
        {
            Console.WriteLine("---Main menu---");
            var mutationChance = 0.01;
            var selectionStyle = SelectionStyle.ROULETTE;
            var crossoverStyle = CrossoverStyle.GENE_CROSSOVER;
            var entityCount = 100;

            Console.Write("Mutation chance (0.01): ");
            try
            {
                var mutation = double.Parse(Console.ReadLine());
                if (mutation >= 0 && mutation <= 1)
                    mutationChance = mutation;
                else
                    Console.WriteLine("Out of range, mutation can be between 0 and 1");
            }
            catch (Exception) { Console.WriteLine("Wrong value for mutatuion, default value will be used. (0.01)"); }


            Console.Write("Selection style [Roulette, Tournament] (ROULETTE): ");
 
                var selection = Console.ReadLine().ToLower();
                if (selection == "roulette")
                    selectionStyle = SelectionStyle.ROULETTE;
                else if(selection == "tournament")
                    selectionStyle = SelectionStyle.TOURNAMENT;
                else if(!string.IsNullOrEmpty(selection))
                Console.WriteLine("Unknown selection style");

            Console.Write("Gene crossover style [Crossover, Genecrossover] (GENECROSSOVER): ");

            var crossover = Console.ReadLine().ToLower();
            if (crossover == "crossover")
                crossoverStyle = CrossoverStyle.CROSSOVER;
            else if (crossover == "genecrossover")
                crossoverStyle = CrossoverStyle.GENE_CROSSOVER;
            else if (!string.IsNullOrEmpty(crossover))
                Console.WriteLine("Unknown crossover style");

            Console.Write("Entity count: (100): ");

            try
            {
                var entity = int.Parse(Console.ReadLine());
                if (entity >= 20 && entity < 500000)
                    entityCount = entity;
                else
                    Console.WriteLine("Entity count has to be larger than or equal to 20 and smaller than 500000");
            }
            catch (Exception) { Console.WriteLine("Wrong value for entity count"); }

            return (mutationChance, selectionStyle, crossoverStyle, entityCount);

        }

        //Loads in a map from a file
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
