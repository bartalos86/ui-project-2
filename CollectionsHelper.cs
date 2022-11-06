namespace genetic_algorithm
{
    public static class CollectionsHelper
    {
        public static bool ContainsByHash<T>(this List<T> list, T item)
        {
            foreach(var listItem in list)
            {
                if (listItem.GetHashCode() == item.GetHashCode())
                    return true;
            }

            return false;
                
        }

    }
}
