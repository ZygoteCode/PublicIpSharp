using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace PublicIpSharpLib
{
    public class PublicIpSharp
    {
        public static string GetPublicIPAddress()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.MaxServicePoints = int.MaxValue;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request1 = (HttpWebRequest)WebRequest.Create($"https://api.seeip.org/");

            request1.Proxy = null;
            request1.UseDefaultCredentials = false;
            request1.AllowAutoRedirect = false;
            request1.Timeout = 70000;

            var field1 = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);

            request1.Method = "GET";

            var headers1 = new CustomWebHeaderCollection(new Dictionary<string, string>
            {
                ["Host"] = $"api.seeip.org"
            });

            field1.SetValue(request1, headers1);

            var response1 = request1.GetResponse();
            string content1 = Encoding.UTF8.GetString(ReadFully(response1.GetResponseStream()));

            response1.Close();
            response1.Dispose();

            return content1;
        }

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }

    internal class CustomWebHeaderCollection : WebHeaderCollection
    {
        private readonly Dictionary<string, string> _customHeaders;

        public CustomWebHeaderCollection(Dictionary<string, string> customHeaders)
        {
            _customHeaders = customHeaders;
        }

        public override string ToString()
        {
            return string.Join("\r\n", _customHeaders.Select(kvp => $"{kvp.Key}: {kvp.Value}").Concat(new[] { string.Empty, string.Empty }));
        }
    }
}