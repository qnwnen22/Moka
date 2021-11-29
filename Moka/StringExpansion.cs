using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExpansion
    {
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
        public static string Push(this string text, string value, Include include = Include.None)
        {
            if (string.IsNullOrEmpty(text)) { return null; }
            if (string.IsNullOrEmpty(value)) { return null; }
            int indexOf = text.IndexOf(value);
            if (indexOf < 0) { return null; }
            int startIndex = indexOf + value.Length;
            string substring = text.Substring(startIndex);
            switch (include)
            {
                case Include.Include:
                    return value + substring;
                default:
                    return substring;
            }
        }
        public static string Cut(this string text, string value, Include include = Include.None)
        {
            if (string.IsNullOrEmpty(text)) { return null; }
            if (string.IsNullOrEmpty(value)) { return null; }
            int indexOf = text.IndexOf(value);
            if (indexOf < 0) { return null; }
            string substring = text.Substring(0, indexOf);
            switch (include)
            {
                case Include.Include:
                    return substring + value;
                default:
                    return substring;
            }
        }
        public static string Truncate(this string text, string startWord, string endWord, Includes includes = Includes.None)
        {
            string push = text.Push(startWord);
            if (string.IsNullOrEmpty(push)) { return null; }
            string cut = push.Cut(endWord);
            if (string.IsNullOrEmpty(cut)) { return null; }
            switch (includes)
            {
                case Includes.Start:
                    return startWord + cut;
                case Includes.End:
                    return cut + endWord;
                case Includes.All:
                    return startWord + cut + endWord;
                default: return cut;
            }
        }
        public static List<string> TruncateList(this string text, string startWord, string endWord, Includes includes = Includes.None)
        {
            var list = new List<string>();
            while (true)
            {
                string push = text.Push(startWord);
                if (string.IsNullOrWhiteSpace(push)) { break; }
                var cut = push.Cut(endWord);
                if (string.IsNullOrWhiteSpace(cut)) { break; }
                text = text.Push(cut + endWord);
                if (string.IsNullOrWhiteSpace(text)) { break; }
                switch (includes)
                {
                    case Includes.Start:
                        list.Add(startWord + cut);
                        break;
                    case Includes.End:
                        list.Add(cut + endWord);
                        break;
                    case Includes.All:
                        list.Add(startWord + cut + endWord);
                        break;
                    default:
                        list.Add(cut);
                        break;
                }
            }
            if (list.IsNullOrEmpty()) { list = null; }
            return list;
        }
        public static List<string> Split(this string text, string value,
            StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return text.Split(new string[] { value }, stringSplitOptions).ToList();
        }
        public static string ReplaceUrlHexEscape(this string text)
        {
            char[] testArray = { '!', '(', ')', '*', '-', '.', '_' };
            foreach (char value in testArray)
            {
                if (text.Contains(value))
                {
                    string hexEscape = Uri.HexEscape(value);
                    text = text.Replace(value.ToString(), hexEscape);
                }
            }
            return text;
        }
        public static string ToUrlEncode(this string text, int count = 1)
        {
            string result = text;
            for (int i = 0; i < count; i++)
            {
                result = WebUtility.UrlEncode(result);
            }
            return result;
        }
        public static string ToUrlDecode(this string text, int count = 1)
        {
            string result = text;
            for (int i = 0; i < count; i++)
            {
                result = WebUtility.UrlDecode(result);
            }
            return result;
        }
        public static string ToBase64Encode(this string text, Encoding encoding = null, int count = 1)
        {
            if (encoding == null) { encoding = Encoding.UTF8; }
            for (int i = 0; i < count; i++)
            {
                byte[] getBytes = encoding.GetBytes(text);
                text = Convert.ToBase64String(getBytes);
            }
            return text;
        }
        public static string ToBase64Decode(this string text, Encoding encoding = null, int count = 1)
        {
            if (encoding is null) { encoding = Encoding.UTF8; }
            for (int i = 0; i < count; i++)
            {
                try
                {
                    byte[] fromBase64String = Convert.FromBase64String(text);
                    text = encoding.GetString(fromBase64String);
                }
                catch { return text; }
            }
            return text;
        }
        public static byte[] GetImageBytes(this string imageAddress)
        {
            try
            {
                Uri uri = new Uri(imageAddress);
                if (uri.IsFile) { imageAddress = "https:" + imageAddress; }
                WebRequest webRequest = WebRequest.Create(imageAddress);
                HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
                if (httpWebRequest is null) { return null; }
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                using (WebResponse webResponse = httpWebRequest.GetResponse())
                {
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception) { return null; }
        }
        public static List<HtmlNode> GetHtmlNodeList(this string html, string xpath)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            HtmlNodeCollection selectNodes = htmlDocument.DocumentNode.SelectNodes(xpath);
            if (selectNodes is null) { return null; }
            IEnumerable<HtmlNode> cast = selectNodes.Cast<HtmlNode>();
            List<HtmlNode> htmlNodeList = cast.ToList();
            return htmlNodeList;

        }
        public static List<string> GetHtmlAttributeValueList(this string html, string xpath, HtmlType htmlType)
        {
            List<HtmlNode> getHtmlNodeList = html.GetHtmlNodeList(xpath);
            if (getHtmlNodeList.IsNullOrEmpty()) { return null; }
            IEnumerable<string> select = null;
            switch (htmlType)
            {
                case HtmlType.InnerHtml:
                    select = getHtmlNodeList.Select(x => x.InnerHtml);
                    break;
                case HtmlType.OuterHtml:
                    select = getHtmlNodeList.Select(x => x.OuterHtml);
                    break;
                case HtmlType.InnerText:
                    select = getHtmlNodeList.Select(x => x.InnerText);
                    break;
            }
            List<string> list = select.ToList();
            return list;
        }
        public static string GetHtmlNodeAttributeValue(this string text, string xPath, string value)
        {
            List<HtmlNode> htmlNodeList = text.GetHtmlNodeList(xPath);
            if (htmlNodeList.IsNullOrEmpty() == true) { return null; }
            HtmlNode htmlNodeFirst = htmlNodeList.First();
            IEnumerable<HtmlAttribute> htmlNodeAttributeIEnumerable = htmlNodeFirst.Attributes.Cast<HtmlAttribute>();
            List<HtmlAttribute> htmlNodeAttributeList = htmlNodeAttributeIEnumerable.ToList();
            HtmlAttribute htmlAttribute = htmlNodeAttributeList.Find(x => x.Name == value);
            if (htmlAttribute is null) { return null; }
            return htmlAttribute.Value;
        }
        public static string GetHtmlNodeAttributeValue(this string text, string xPath, HtmlType htmlType)
        {
            List<HtmlNode> htmlNodeList = text.GetHtmlNodeList(xPath);
            HtmlNode htmlNodeFirst = htmlNodeList.First();
            switch (htmlType)
            {
                case HtmlType.InnerText:
                    return htmlNodeFirst.InnerText;
                case HtmlType.InnerHtml:
                    return htmlNodeFirst.InnerHtml;
                case HtmlType.OuterHtml:
                    return htmlNodeFirst.OuterHtml;
                default: return null;
            }
        }
        public static List<string> GetHtmlNodeAttributeValueList(this string text, string xPath, string value)
        {
            List<HtmlNode> htmlNodeList = text.GetHtmlNodeList(xPath);

            List<string> result = new List<string>();
            for (int i = 0; i < htmlNodeList.Count; i++)
            {
                IEnumerable<HtmlAttribute> htmlAttributeIEnumerable = htmlNodeList[i].Attributes.Cast<HtmlAttribute>();
                List<HtmlAttribute> htmlAttributeList = htmlAttributeIEnumerable.ToList();
                HtmlAttribute htmlAttribute = htmlAttributeList.Find(x => x.Name == value);
                if (htmlAttribute is null) { continue; }
                result.Add(htmlAttribute.Value);
            }
            return result;
        }
        public static List<string> GetHtmlNodeAttributeValueList(this string text, string xPath, HtmlType htmlType)
        {
            List<HtmlNode> htmlNodeList = text.GetHtmlNodeList(xPath);

            List<string> result = new List<string>();
            if (htmlNodeList.IsNullOrEmpty()) { return null; }
            for (int i = 0; i <= htmlNodeList.Count - 1; i++)
            {
                switch (htmlType)
                {
                    case HtmlType.InnerText:
                        result.Add(htmlNodeList[i].InnerText.Trim());
                        break;
                    case HtmlType.InnerHtml:
                        result.Add(htmlNodeList[i].InnerHtml.Trim());
                        break;
                    case HtmlType.OuterHtml:
                        result.Add(htmlNodeList[i].OuterHtml.Trim());
                        break;
                }
            }
            return result;
        }
        private static T DeserializeObject<T>(this string json, JsonSerializerSettings jsonSerializerSettings)
        {
            T deserializeObject = JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
            return deserializeObject;
        }
        public static T ToClass<T>(this string json)
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            jsonSerializerSettings.Error = (se, ev) => { ev.ErrorContext.Handled = true; };
            T deserializeObject = json.DeserializeObject<T>(jsonSerializerSettings);
            return deserializeObject;
        }
        private static string ToUrlHexEscape(this string text)
        {
            char[] testArray = { '!', '(', ')', '*', '-', '.', '_' };
            foreach (var value in testArray)
            {
                if (text.Contains(value) == true)
                {
                    string hexEscape = Uri.HexEscape(value);
                    text = text.Replace(value.ToString(), hexEscape);
                }
            }
            return text;
        }

    }
}
