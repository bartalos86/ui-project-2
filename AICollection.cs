﻿
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
        private Func<double> goldWorth = () => 25;

        private Action selectionFunction;
        private Func<VirtualMachine, VirtualMachine, VirtualMachine> crossoverFunction;

        public AICollection(double chanceOfMutation = 0.01, SelectionStyle selectionStyle = SelectionStyle.ROULETTE,
            CrossoverStyle crossoverStyle = CrossoverStyle.GENE_CROSSOVER)
        {
            VirtualMachines = new List<VirtualMachine>();
            ChanceOfMutation = chanceOfMutation;
            Generation = 0;

            if(selectionStyle == SelectionStyle.ROULETTE)
            {
                selectionFunction = DoRuletteEvolution;
            }
            else
            {
                selectionFunction = DoTournamentEvolution;
            }

            if(crossoverStyle == CrossoverStyle.GENE_CROSSOVER)
            {
                crossoverFunction = GeneCrossover;
            }
            else
            {
                crossoverFunction = Crossover;
            }
        }

        public void InitializeAIs(int count = 20)
        {
            MaxAIs = count; //> 20 ? count /2 : count;
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

            selectionFunction.Invoke();
        }

        private void DoTournamentEvolution()
        {
            var aisInTournament = new List<VirtualMachine>();
            var random = new Random();

           

            if (!SolutionFound)
            {
                double totalFitness = 0;
                foreach (var vm in VirtualMachines)
                    totalFitness += vm.Fitness;

                Console.Clear();
                if(VirtualMachines[0].Steps  != null)
                Map.PrintOutPath(VirtualMachines[0].Steps);
                Console.WriteLine($"Average fitness: {Math.Round((totalFitness / VirtualMachines.Count), 1)}");
                Console.WriteLine($"Best fitness: {Math.Round(VirtualMachines[0].Fitness, 1)}");
                Console.WriteLine($"Current generation: {Generation}");
                System.Console.WriteLine($"Number of AIs: {VirtualMachines.Count}");
            }

   

            var requiredAiCount = Math.Max(MaxAIs / 3, Math.Sqrt(MaxAIs)+3);
            Console.WriteLine($"Required ai cound: {requiredAiCount}");

            for(int i = 0; aisInTournament.Count < requiredAiCount; i++)
            {
                var ai = VirtualMachines[random.Next(VirtualMachines.Count)];
                if (!aisInTournament.Contains(ai))
                    aisInTournament.Add(ai);
            }

            aisInTournament.Sort();
            aisInTournament.Reverse();

            var maxForNewChildren = Math.Ceiling(Math.Sqrt(MaxAIs)) + 5;

            var newGeneration = new List<VirtualMachine>();

            //Elits for only mutation
            for (int i = 0, counter = 0; i < 3; i++)
            {
                if (!newGeneration.Contains(VirtualMachines[counter]))
                {
                    var vm = VirtualMachines[counter++];
                    vm.Memory = Mutate(vm.Memory);
                    newGeneration.Add(vm);

                }
                else
                    i--;

            }

            for (int i = 0; i< maxForNewChildren; i++)
            {
                for (int j = i+1; j < maxForNewChildren; j++)
                {
                    var newChild = crossoverFunction.Invoke(aisInTournament[i], aisInTournament[j]);
                    newGeneration.Add(newChild);

                    if (newGeneration.Count >= MaxAIs)
                        break;
                }

                if (newGeneration.Count >= MaxAIs)
                    break;
            }

            VirtualMachines = newGeneration;

        }

        private void DoRuletteEvolution()
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
                Console.WriteLine($"Average fitness: {Math.Round((totalFitness / VirtualMachines.Count), 1)}%");
                Console.WriteLine($"Best fitness: {Math.Round(VirtualMachines[0].Fitness, 1)}");
                Console.WriteLine($"Current generation: {Generation}");
                System.Console.WriteLine($"Number of AIs: {VirtualMachines.Count}");
            }
          

            foreach (var vm in VirtualMachines)
            {
                double chance = ((vm.Fitness / totalFitness)) * 100;
                chanceDictionary.Add((chance, vm));
            }

            var random = new Random();
            var successfulAIs = new List<VirtualMachine>();

             //Elits for crossover
            for(int i  = 0, counter = 0; i < Math.Max(Math.Sqrt(MaxAIs)/2,5); i++)
            {
                if (!successfulAIs.Contains(VirtualMachines[counter]))
                    successfulAIs.Add(VirtualMachines[counter++]);
                    else
                    i--;
                    
            }

            //Lottery
            int elitCount = successfulAIs.Count;
            for(int i = 0; i< MaxAIs - elitCount; i++)
            {
                var number = random.NextDouble() *100;
                int counter = 0;
                var element = chanceDictionary[counter];
                double total = element.Item1;

                while (total < number)
                {
                    // if (counter >= chanceDictionary.Count)
                    //     break;

                    element = chanceDictionary[counter++];
                    total += element.Item1;
                }
                if (!successfulAIs.Contains(element.Item2))
                {
                    successfulAIs.Add(element.Item2);
                }
               
            }

           var newGeneration = new List<VirtualMachine>();

            //Elits for only mutation
            for (int i = 0, counter = 0; i < Math.Max(Math.Sqrt(MaxAIs) / 2, 5); i++)
            {
                if (!newGeneration.Contains(VirtualMachines[counter]))
                {
                    var vm = VirtualMachines[counter++];
                    vm.Memory = Mutate(vm.Memory);
                    newGeneration.Add(vm);

                }
                else
                    i--;

            }


            successfulAIs.Sort();
            successfulAIs.Reverse();

            //Fill out with normals
            for (int i = 0; successfulAIs.Count < Math.Ceiling(Math.Sqrt(MaxAIs))+3; i++)
            {
                if (!successfulAIs.Contains(VirtualMachines[i]))
                {
                    var vm = VirtualMachines[i];
                    successfulAIs.Add(vm);

                }

            }
            int maxCap = successfulAIs.Count <= MaxAIs ? successfulAIs.Count : MaxAIs;


            Console.WriteLine($"Succesful count: {successfulAIs.Count}");


            for (int i = 0; i < maxCap; i++)
            {
                //Console.WriteLine("Fitness: " + successfulAIs[i].Fitness);

                for (int j = i + 1;j < maxCap; j++)
                {
                    var newChild = crossoverFunction.Invoke(successfulAIs[i], successfulAIs[j]);
                    if(newGeneration.Count >= MaxAIs){
                        break;
                    }
                    newGeneration.Add(newChild);
                }

                if(newGeneration.Count >= MaxAIs){
                        break;
                    }
            }

            Console.WriteLine($"New generation count: {newGeneration.Count}");


            VirtualMachines = newGeneration;
                

        }

        private VirtualMachine GeneCrossover(VirtualMachine parent1, VirtualMachine parent2)
        {
            var child = new VirtualMachine(ControlBounds);
            var crossoverMemory = new List<Memory>();
            var totalFitness = parent1.Fitness + parent2.Fitness;
            var random = new Random();

            for (int i = 0; i < parent1.Memory.Count; i++)
            {
                if(random.NextDouble() < (parent1.Fitness / totalFitness))
                {
                    crossoverMemory.Add(parent1.Memory[i].DeepClone());
                }
                else
                {
                    crossoverMemory.Add(parent2.Memory[i].DeepClone());

                }

            }

            child.Memory = crossoverMemory.DeepClone();

            child.Memory = Mutate(child.Memory);

            return child;
        }

        private VirtualMachine Crossover(VirtualMachine parent1, VirtualMachine parent2)
        {
            var child = new VirtualMachine(ControlBounds);
            var crossoverMemory = new List<Memory>();
            var totalFitness = parent1.Fitness + parent2.Fitness;

            int crossoverPoint = (int)Math.Floor(parent1.Memory.Count * (parent1.Fitness / totalFitness));

            for(int i = 0; i < parent1.Memory.Count; i++)
            {
                if(i >= crossoverPoint)
                    crossoverMemory.Add(parent2.Memory[i].DeepClone());
                else
                    crossoverMemory.Add(parent1.Memory[i].DeepClone());

            }

            child.Memory = crossoverMemory;

            child.Memory = Mutate(child.Memory);

            return child;
        }

        private List<Memory> Mutate(List<Memory> completeMemory)
        {
            var random = new Random();
            var newMemory = new List<Memory>();
            
            foreach(var memory in completeMemory)
            {
                for(int i = 0; i < 8; i++)
                {
                    
                    if (random.NextDouble() <= ChanceOfMutation)
                    {
                        //Console.WriteLine($"Before {memory.Value}");

                        if (!memory.Value.IsOneAtPosition(i))
                        {
                            memory.Value = (memory.Value | (uint)Math.Pow(2, 8 - i - 1));
                        }
                        else
                            memory.Value = (memory.Value & (255-(uint)Math.Pow(2, 8-i-1)));

                        //Console.WriteLine($"After {memory.Value}");
                    }
                }
                newMemory.Add(memory);
            }

            return newMemory;
        }

        private double FitnessFunction(VirtualMachine machine) {

            double score = 0.01;
            if (machine.Steps == null)
                return 0.01;

            var pathData = Map.PerfromsSteps(machine.Steps);
            var position = pathData.Item1;
            var goldsCollected = pathData.Item2;
            bool wereNotCollectedGolds = false;

            //var rightSteps = machine.Steps.FindAll((step) => step == Step.P);
            //score -= rightSteps.Count * 0.1;
            //score -= machine.Steps.FindAll((step) => step == Step.H).Count * 0.1;

            // foreach (var gold in goldsCollected)
            //{
            score = Math.Pow(goldWorth.Invoke(),goldsCollected.Count);
            //score = goldsCollected.Count * goldWorth.Invoke();
            var stepPenalty = Math.Pow(1.45, machine.Steps.Count / 2);

            if(score - stepPenalty > 0.1)
                score = score - stepPenalty;

            // }

            foreach (var goldPosition in Map.GoldPositions)
            {
                if (goldsCollected.ContainsByHash(goldPosition))
                    continue;

                score += Math.Pow(1.40,goldWorth.Invoke() / (Map.GetDistance(goldPosition, position)+1));
                wereNotCollectedGolds = true;
            }

            if( goldsCollected.Count == Map.GoldPositions.Count)
            {
                Console.Clear();
                Console.WriteLine("Final AI found");
                Console.WriteLine($"Best fitness: {Math.Round(machine.Fitness, 1)}");
                System.Console.WriteLine($"Number of AIs: {VirtualMachines.Count}");
                Console.WriteLine($"Total generations: {Generation}");
                Map.PrintOutPath(machine.Steps);
                SolutionFound = true;
            }

            return score;

        }

    }
}
