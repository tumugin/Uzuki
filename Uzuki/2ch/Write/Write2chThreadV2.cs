using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Uzuki.Network;

namespace Uzuki._2ch.Write
{
    public class Write2chThreadV2
    {
        private const String WRITE_USER_AGENT = "Uzuki/1.0";
        private static Encoding WriteEncoding = Encoding.GetEncoding("Shift-JIS");
        private const String WRITE = "書き込む";
        private String ThreadBBSName;
        private String ThreadBBSKey;
        private String ThreadBBSHost;
        private CookieContainer cookiecontainer;

        public Write2chThreadV2(String bbs,String key,String host,CookieContainer ck)
        {
            ThreadBBSName = bbs;
            ThreadBBSKey = key;
            ThreadBBSHost = host;
            cookiecontainer = ck;
        }

        public void Write(String name,String mail,String message)
        {
            //make write params
            String req = string.Concat(new string[]
            {
                "bbs=",
                ThreadBBSName,
                "&key=",
                ThreadBBSKey,
                "&time=",Tools.UnixTime.ToUnixTime(DateTime.Now).ToString(),
                "&submit=",
                HttpUtility.UrlEncode(WRITE, WriteEncoding),
                "&FROM=",
                HttpUtility.UrlEncode(name, WriteEncoding),
                "&mail=",
                HttpUtility.UrlEncode(mail, WriteEncoding),
                "&MESSAGE=",
                HttpUtility.UrlEncode(message, WriteEncoding)
            });
            byte[] encoded = Encoding.ASCII.GetBytes(req);
            //generate referer
            String referer = "http://" + ThreadBBSHost + "/" + ThreadBBSKey;
            //make request
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create("http://" + ThreadBBSHost + "/test/bbs.cgi?guid=ON");
            httpreq.Method = "POST";
            httpreq.ContentType = "application/x-www-form-urlencoded";
            httpreq.CookieContainer = cookiecontainer;
            httpreq.Referer = referer;
            httpreq.UserAgent = WRITE_USER_AGENT;
            httpreq.ContentLength = encoded.LongLength;
            //write to request stream
            Stream st = httpreq.GetRequestStream();
            st.Write(encoded, 0, encoded.Length);
            st.Close();
            //get return value
            HttpWebResponse response = (HttpWebResponse)httpreq.GetResponse();
            //copy stream
            MemoryStream mems = Ayane.CopyAndClose(response.GetResponseStream());
            byte[] context = mems.ToArray();
            mems.Close();
            //get encoding
            Encoding encode = Ayaka.GetCode(context);
            String html = encode.GetString(context);
            Debug.WriteLine(html);
            //以下エラー判定
            if (!html.Contains("書きこみました"))
            {
                //書き込めていない
                //エラー文字列をHTMLからテキストに変換する
                Regex re = new Regex("<.*?>", RegexOptions.Singleline);
                Regex br = new Regex("<br>", RegexOptions.Singleline);
                string output = br.Replace(html, Environment.NewLine);
                output = re.Replace(output, "");
                throw new Exception(output);
            }
            
        }

        //Exception
        public class Write2chWriteException : Exception { }
    }
}
