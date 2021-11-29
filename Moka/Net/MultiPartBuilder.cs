using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    public class MultiPartBuilder : IDisposable
    {
        public class Row
        {
            public readonly string Name;
            public readonly string Value;
            public readonly string Filename;
            public readonly string ContentType;
            public readonly string Path;
            public readonly byte[] Bytes;
            public Row(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
            public Row(string name, string filename, string contentType, string path)
            {
                this.Name = name;
                this.Filename = filename;
                this.ContentType = contentType;
                this.Path = path;
            }
            public Row(string name, string filename, string contentType, byte[] bytes)
            {
                this.Name = name;
                this.Filename = filename;
                this.ContentType = contentType;
                this.Bytes = bytes;
            }
        }
        public readonly string boundary;
        public List<Row> Rows { get; private set; }
        public MultiPartBuilder(string boundary)
        {
            this.boundary = boundary;
            this.Rows = new List<Row>();
        }
        public void Add(string name, string value)
        {
            this.Rows.Add(new Row(name, value));
        }
        public void Add(string name, string filename, string contentType, string path = null)
        {
            this.Rows.Add(new Row(name, filename, contentType, path));
        }
        public void Add(string name, string filename, string contentType, byte[] bytes = null)
        {
            this.Rows.Add(new Row(name, filename, contentType, bytes));
        }

        public enum ImageType
        {
            File,
            Bytes
        }


        public byte[] GetBytes(Encoding encoding, ImageType imageType)
        {
            byte[] result = null;
            try
            {
                var postBytes = new List<byte>();
                for (int i = 0; i <= Rows.Count - 1; i++)
                {
                    if (Rows[i].ContentType == null)
                    {
                        var sb = new StringBuilder();
                        {
                            sb.Append("--" + this.boundary); sb.Append(Environment.NewLine);
                            sb.Append("Content-Disposition: form-data; name=\"" + Rows[i].Name + "\""); sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);
                            sb.Append(Rows[i].Value);
                            sb.Append(Environment.NewLine);
                            byte[] bytes = encoding.GetBytes(sb.ToString());
                            postBytes.AddRange(bytes.ToList().GetRange(0, bytes.Length));
                        }
                    }
                    else
                    {
                        if (Rows[i].ContentType == "application/octet-stream") { }
                        var sb = new StringBuilder();
                        {
                            sb.Append("--" + this.boundary); sb.Append(Environment.NewLine);
                            sb.Append("Content-Disposition: form-data; name=\"" + Rows[i].Name + "\"; filename=\"" + Rows[i].Filename + "\"");
                            sb.Append(Environment.NewLine);
                            sb.Append("Content-Type: " + Rows[i].ContentType); sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);

                            byte[] bytes = Encoding.Default.GetBytes(sb.ToString());
                            postBytes.AddRange(bytes.ToList().GetRange(0, bytes.Length));
                        }
                        switch (imageType)
                        {
                            case ImageType.File:
                                if (Rows[i].Path != null)
                                {
                                    switch (Rows[i].Path.IsNullOrWhiteSpace())
                                    {
                                        case true:
                                            sb.Append("--" + this.boundary);
                                            sb.Append(Environment.NewLine);
                                            sb.Append("Content-Disposition: form-data; name=\"" + Rows[i].Name + "\"; filename=\"" + Rows[i].Filename + "\"");
                                            sb.Append(Environment.NewLine);
                                            sb.Append("Content-Type: " + Rows[i].ContentType);
                                            sb.Append(Environment.NewLine);
                                            sb.Append("");
                                            sb.Append(Environment.NewLine);
                                            break;
                                        case false:
                                            using (var fileStream = new FileStream(Rows[i].Path, FileMode.Open, FileAccess.Read))
                                            {
                                                byte[] fileBytes = new byte[fileStream.Length];
                                                fileStream.Read(fileBytes, 0, fileBytes.Length);
                                                postBytes.AddRange(fileBytes.ToList().GetRange(0, fileBytes.Length));
                                                fileStream.Close();
                                            }
                                            break;
                                    }
                                }
                                break;
                            case ImageType.Bytes:
                                if (Rows[i].Bytes != null)
                                {
                                    postBytes.AddRange(Rows[i].Bytes.ToList().GetRange(0, Rows[i].Bytes.Length));
                                }
                                break;
                        }
                        byte[] fileLastLine = encoding.GetBytes(Environment.NewLine);
                        postBytes.AddRange(fileLastLine.ToList().GetRange(0, fileLastLine.Length));
                    }
                }
                byte[] lastBytes = encoding.GetBytes("--" + this.boundary + "--");
                postBytes.AddRange(lastBytes.ToList().GetRange(0, lastBytes.Length));
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(postBytes.ToArray(), 0, postBytes.Count);
                    memoryStream.Position = 0;
                    result = new byte[memoryStream.Length];
                    memoryStream.Read(result, 0, result.Length);
                    memoryStream.Close();
                }
            }
            catch (Exception) { }
            //string test =encoding.GetString(result.ToArray());
            return result;
        }
        //public byte[] GET_BYTE_ARRAY()
        //{
        //    return this.GetBytes(Encoding.Default, ImageType.File);
        //}
        //public byte[] GetByteArray(Encoding encoding)
        //{
        //    return this.GetBytes(encoding, ImageType.File);
        //}
        public void Dispose() { }
    }
}
