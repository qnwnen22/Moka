using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    public static class HttpWebRequestException
    {
        public static void Write(this HttpWebRequest httpWebRequest, byte[] bytes)
        {
            using (Stream stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        public static void Write(this HttpWebRequest httpWebRequest, string text, Encoding encoding = null)
        {
            if (encoding is null) { encoding = Encoding.UTF8; }
            byte[] getBytes = encoding.GetBytes(text);
            httpWebRequest.Write(getBytes);
        }
        public static string ReadToEnd(this HttpWebRequest httpWebRequest, Encoding encoding = null)
        {
            if (encoding is null) { encoding = Encoding.UTF8; }
            using (WebResponse webResponse = httpWebRequest.GetResponse())
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(stream, encoding))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
        public static T ReadToEnd<T>(this HttpWebRequest httpWebRequest, Encoding encoding = null)
        {
            string readToEnd = httpWebRequest.ReadToEnd(encoding);
            if (string.IsNullOrWhiteSpace(readToEnd)) { return default; }
            T toClass = readToEnd.ToClass<T>();
            return toClass;
        }
    }
}
