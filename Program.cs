using System.Text;

namespace genetic_algorithm
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            //VirtualMachine virtualMachine = new VirtualMachine();
            //virtualMachine.PrintOutMemory();
            //virtualMachine.DoGeneration();
            AICollection swarm = new AICollection();
            swarm.InitializeAIs(20);

            Map startingMap = ReadMap("testMap.txt");
            startingMap.PrintMap();
            swarm.SetupMap(startingMap);
            swarm.DoGeneration();


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
