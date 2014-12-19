using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace HousingData.Server
{
    public class HousingDataDownloader
    {
        public static void DownloadIfNotThere(string fileName)
        {
            var uri = ConfigurationManager.AppSettings["DataLocation"];
            if (string.IsNullOrEmpty(uri))
            {
                throw new InvalidOperationException("Invalid uri configured for housing data file");
            }

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists || fileInfo.Length == 0)
            {
                var webRequest = WebRequest.Create(uri);
                webRequest.Method = "GET";
                using (var file = File.Open(fileName, FileMode.Create))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        using (var webResponse = webRequest.GetResponse())
                        {
                            using (var stream = webResponse.GetResponseStream())
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var buffer = new char[1024];
                                        var bytesRead = reader.Read(buffer, 0, buffer.Length);
                                        writer.Write(buffer, 0, bytesRead);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
