using System.Linq;

namespace Utils.Extensions
{
    public static class ArrayExtension
    {
        public static bool IsEqual<T>(this T[] array1, T[] array2)
        {
            return (array1.Count() == array2.Count()) && !array1.Except(array2).Any() && !array2.Except(array1).Any();
        }
    }
}
