
namespace genetic_algorithm
{
    public class AICollection
    {
        public List<VirtualMachine> VirtualMachines { get; set; }
        public Map Map { get; private set; }
        public int Generation { get; private set; }
        public int MutationAmount { get; set; }

        public AICollection()
        {
            VirtualMachines = new List<VirtualMachine>();
        }

        public void InitializeAIs(int count = 20)
        {
            for(int i = 0; i < count; i++)
            {
                VirtualMachines.Add(new VirtualMachine(ControlBounds));
            }
        }

        private bool ControlBounds(List<Step> steps)
        {
            return Map.IsInside(steps);
        }

        public void SetupMap(Map map)
        {
            Map = map;
        }

        public void DoGeneration()
        {
            foreach(var ai in VirtualMachines)
            {
                ai.DoGeneration(FitnessFunction);
                Console.WriteLine(ai.Fitness);
            }


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
                score += 10 * Map.Width/2;
                Console.WriteLine(gold);
            }

            foreach(var goldPosition in Map.GoldPositions)
            {
                if (goldsCollected.Contains(goldPosition))
                    continue;

                score += Math.Sqrt(Math.Pow(Map.Width, 2) + Math.Pow(Map.Height, 2)) / (Map.GetDistance(goldPosition, position)+1);
            }

            return score;

        }

    }
}
