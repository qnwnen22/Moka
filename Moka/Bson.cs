using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Bson
    {
        public static string GetID()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
