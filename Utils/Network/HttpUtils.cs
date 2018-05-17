using System.IO;
using System.Net;
using System.Text;

namespace Utils.Network
{
    public static class HttpUtils
    {
        public static string DownloadHtml(string urlAddress)
        {
            var request = (HttpWebRequest) WebRequest.Create(urlAddress);
            using (var response = (HttpWebResponse) request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var receiveStream = response.GetResponseStream();
                    if (receiveStream == null)
                    {
                        return null;
                    }

                    using (var readStream = response.CharacterSet == null
                        ? new StreamReader(receiveStream)
                        : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)))
                    {
                        return readStream.ReadToEnd();
                    }
                }
                
                return null;
            }
        }
    }
}