using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ObjectExpansion
    {
        private static string SerializeObject(this object value)
        {
            string serializeObject = JsonConvert.SerializeObject(value);
            return serializeObject;
        }
        public static string ToJson(this object value)
        {
            string serializeObject = value.SerializeObject();
            if (serializeObject == "null") { return null; }
            return serializeObject;
        }
    }
}
