using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class QueryStringBuilder
    {
        private readonly List<string> list = new List<string>();
        public void Add(string text)
        {
            this.list.Add(text);
        }
        public void Add(string key, string value)
        {
            this.list.Add($"{key}={value}");
        }
        public new string ToString()
        {
            var result = this.list.Join("&");
            this.list.Clear();
            return result;
        }
        public byte[] GetBytes(Encoding encoding = null)
        {
            if(encoding is null) { encoding = Encoding.UTF8; }
            var toString = this.ToString();
            return encoding.GetBytes(toString);
        }
    }
}
