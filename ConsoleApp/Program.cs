using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            string address = "https://www.google.com/";

            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(address);
            httpWebRequest.Method = "GET";
            var readToEnd = httpWebRequest.ReadToEnd();

            var path = Directory.GetCurrentDirectory() + "\\html.txt";
            var file = File.ReadAllText(path);

            var a = file.GetHtmlNodeAttributeValueList("//div", "class");

        }
    }
}
