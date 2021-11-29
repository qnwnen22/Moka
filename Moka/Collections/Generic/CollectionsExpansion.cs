using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class CollectionsExpansion
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> iEnumerable)
        {
            return (iEnumerable is null || iEnumerable.Count() <= 0);
        }
        public static List<T> RemoveNull<T>(this List<T> tList)
        {
            return tList.FindAll(x => x != null);
        }
        public static List<T> DistinctList<T>(this List<T> tList)
        {
            return tList.Distinct<T>().ToList();
        }
        public static string Join<T>(this List<T> tList, string separator = "")
        {
            return string.Join(separator, tList);
        }
    }
}
