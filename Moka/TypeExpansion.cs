using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class TypeExpansion
    {
        public static List<Type> GetInterfaceTypeList(this Type type)
        {
            Type[] getInterfaces = type.GetInterfaces();
            List<Type> getInterfaceTypeList = getInterfaces.ToList();
            return getInterfaceTypeList;
        }
        public static Type GetInterfaceType(this Type type)
        {
            List<Type> getInterfaceTypeList = type.GetInterfaceTypeList();
            Type firstInterfaceType = getInterfaceTypeList.First();
            return firstInterfaceType;
        }
        public static string CreateClassName(this Type type, string separator = "_")
        {
            if (type.IsGenericType == true)
            {
                Type[] getGenericArguments = type.GetGenericArguments();
                type = getGenericArguments.First();
            }
            List<string> splitTypeFullNameItemList = type.FullName.Split(".");
            string lastTypeFullNameItemList = splitTypeFullNameItemList.Last();
            List<string> splitLastTypeFullNameItemList = lastTypeFullNameItemList.Split("+");
            string joinSplitLastTypeFullNameItemList = splitLastTypeFullNameItemList.Join(separator);
            return joinSplitLastTypeFullNameItemList;
        }
    }
}
