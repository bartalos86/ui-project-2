
namespace genetic_algorithm
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;  
        }

        public override int GetHashCode()
        {
            return $"x:{X}y:{Y}".GetHashCode();
        }
    }
}
