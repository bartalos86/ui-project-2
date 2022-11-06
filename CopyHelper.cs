using System.Runtime.Serialization.Formatters.Binary;


namespace genetic_algorithm
{
    //Helper class made to more easily clone objects in memory without retaining the references to the original nested instances
    //Inspired from stack overflow:https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net
    public static class CopyHelper
    {
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
