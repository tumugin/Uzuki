using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Uzuki.Network
{
    public class Ayane
    {
        public static String getHttp(String URL)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.Method = "GET";
            req.UserAgent = "Uzuki/1.0";
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream webs = res.GetResponseStream();
            MemoryStream mems = CopyAndClose(webs);
            byte[] bt = new byte[mems.Length];
            mems.Read(bt, 0, bt.Length);
            mems.Close();
            Encoding encode = Ayaka.GetCode(bt);
            String content = encode.GetString(bt);
            return content;
        }

        private static MemoryStream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }
    }
}
