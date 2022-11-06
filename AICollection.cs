
using System.Text;
using System.Threading.Channels;

namespace genetic_algorithm
{
    public class AICollection
    {
        public List<VirtualMachine> VirtualMachines { get; set; }
        public Map Map { get; private set; }
        public int Generation { get; private set; }
        public int MutationAmount { get; set; }
        public int MaxAIs { get; set; }
        public double ChanceOfMutation { get; private set; }

        public bool SolutionFound { get; private set; }

        private double maxPossibleFitness;
        private int customSize = 64;
        private Func<double> goldWorth = () => 5;

        public AICollection(double chanceOfMutation = 0.01)
        {
            VirtualMachines = new List<VirtualMachine>();
            ChanceOfMutation = chanceOfMutation;
            Generation = 0;
        }

        public void InitializeAIs(int count = 20)
        {
            MaxAIs = count;
            for (int i = 0; i < count; i++)
            {
                VirtualMachines.Add(new VirtualMachine(ControlBounds, customSize));
            }
        }

        private (bool,int) ControlBounds(List<Step> steps)
        {
            return Map.IsInside(steps);
        }

        public void SetupMap(Map map)
        {
            int mapSize = map.Width * map.Height;
            this.customSize = mapSize > 64 ? mapSize: 64;
            maxPossibleFitness = map.GoldPositions.Count * goldWorth.Invoke();
            Map = map;
        }

        public void DoGeneration()
        {
            Generation++;
            foreach(var ai in VirtualMachines)
            {
                ai.DoGeneration(FitnessFunction);
            }

            DoEvolution();


        }

        private void DoEvolution()
        {
            VirtualMachines.Sort();
            VirtualMachines.Reverse();

            double totalFitness = 0;
            var chanceDictionary = new List<(double, VirtualMachine)>();

            foreach (var vm in VirtualMachines)
                totalFitness += vm.Fitness;

            if (!SolutionFound)
            {
                Console.Clear();
                Map.PrintOutPath(VirtualMachines[0].Steps);
                Console.WriteLine($"Average fitness: {Math.Round((totalFitness / VirtualMachines.Count) / maxPossibleFitness * 100, 1)}%");
                Console.WriteLine($"Best fitness: {Math.Round(VirtualMachines[0].Fitness, 1)}");
                Console.WriteLine($"Current generation: {Generation}");
            }
          

            foreach (var vm in VirtualMachines)
            {
                double chance = ((vm.Fitness / totalFitness)) * 100;
                chanceDictionary.Add((chance, vm));
            }

            var random = new Random();

            var successfulAIs = new List<VirtualMachine>();
            for(int i = 0; i< VirtualMachines.Count; i++)
            {
                var number = random.NextDouble() *100;
                int counter = 0;
                var element = chanceDictionary[counter];
                double total = element.Item1;

                while (total < number)
                {
                    element = chanceDictionary[counter++];
                    total += element.Item1;
                }
                if (!successfulAIs.Contains(element.Item2))
                {
                    successfulAIs.Add(element.Item2);
                }
               
            }

           

            //No AIs were picked
            if(successfulAIs.Count <= 5)
            {
                for (int i = 0; i < VirtualMachines.Count /2; i++)
                    successfulAIs.Add(VirtualMachines[i]);
            }

            int maxCap = successfulAIs.Count <= MaxAIs ? successfulAIs.Count : MaxAIs;

           var newGeneration = new List<VirtualMachine>();
           for(int i = 0; i < maxCap; i++)
            {
                //Console.WriteLine("Fitness: " + successfulAIs[i].Fitness);

                for (int j = i + 1;j < maxCap; j++)
                {
                    var newChild = Crossover(successfulAIs[i], successfulAIs[j]);
                    newGeneration.Add(newChild);
                }
            }

            VirtualMachines = newGeneration;
                

        }

        private VirtualMachine Crossover(VirtualMachine parent1, VirtualMachine parent2)
        {
            var child = new VirtualMachine(ControlBounds);
            var crossoverMemory = new List<Memory>();
            int crossoverPoint = parent1.Memory.Count/2;

            for(int i = 0; i < parent1.Memory.Count; i++)
            {
                if(i >= crossoverPoint)
                    crossoverMemory.Add(parent2.Memory[i].DeepClone());
                else
                    crossoverMemory.Add(parent1.Memory[i].DeepClone());

            }

            child.Memory = crossoverMemory.DeepClone();

            child.Memory = Mutate(child.Memory);

            return child;
        }

        private List<Memory> Mutate(List<Memory> completeMemory)
        {
            var random = new Random();
            foreach(var memory in completeMemory)
            {
                for(int i = 0; i < memory.Value.Length; i++)
                {
                    
                    if (random.NextDouble() <= ChanceOfMutation)
                    {
                        StringBuilder modified = new StringBuilder(memory.Value);

                        if (memory.Value[i] == '0')
                            modified[i] = '1';
                        else
                            modified[i] = '0';

                        memory.Value = modified.ToString();


                    }
                }
            }

            return completeMemory;
        }

        private double FitnessFunction(VirtualMachine machine) {

            double score = 0;
            if (machine.Steps == null)
                return 0;

            var pathData = Map.PerfromsSteps(machine.Steps);
            var position = pathData.Item1;
            var goldsCollected = pathData.Item2;

            foreach(var gold in goldsCollected)
            {
                score += goldWorth.Invoke();
            }

            foreach(var goldPosition in Map.GoldPositions)
            {
                if (goldsCollected.ContainsByHash(goldPosition))
                    continue;

                score += 1 / (Map.GetDistance(goldPosition, position)+1);
            }

            if(score >= maxPossibleFitness)
            {
                Console.Clear();
                Console.WriteLine("Final AI found");
                Console.WriteLine($"Total generations: {Generation}");
                Map.PrintOutPath(machine.Steps);
                SolutionFound = true;
            }

            return score;

        }

    }
}
