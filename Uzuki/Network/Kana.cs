using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Uzuki.Dialogs.MainWindow;

namespace Uzuki.Network
{
    class Kana
    {
        static String AuthKey = null;
        const String AUTHURL = "https://api.2ch.net/v1/auth/";

        public static String get2chDAT(String datURL)
        {
            Uri uri = new Uri(datURL);
            Auth();//認証処理実行
            String server = Regex.Match(uri.Host, @"(.*)(\.2ch\.net|\.bbspink\.com)").Groups[1].Value;
            Match regexpath = Regex.Match(uri.AbsolutePath, @"\/(.*)\/dat\/(.*).dat");
            String boardname = regexpath.Groups[1].Value;
            String ID = regexpath.Groups[2].Value;
            String res = getDAT(server, boardname, ID, AuthKey);
            return res;
        }

        public static String CaculateHMAC(String key,String mesg){
            byte[] data = System.Text.Encoding.UTF8.GetBytes(mesg);
            byte[] keyData = System.Text.Encoding.UTF8.GetBytes(key);
            //HMACSHA1オブジェクトの作成
            System.Security.Cryptography.HMACSHA256 hmac = new System.Security.Cryptography.HMACSHA256(keyData);
            //ハッシュ値を計算
            byte[] bs = hmac.ComputeHash(data);
            //リソースを解放する
            hmac.Clear();
            //byte型配列を16進数に変換
            return BitConverter.ToString(bs).ToLower().Replace("-", "");
        }

        static void Auth(){
            if (AuthKey != null) return;
            String CT = "1234567890";
            String message = SingletonManager.MainWindowSingleton.SetMannage.NetAPIKEY + CT;
            //送信する値
            Hashtable ht = new Hashtable();
            ht["ID"] = "";
            ht["PW"] = "";
            ht["KY"] = SingletonManager.MainWindowSingleton.SetMannage.NetAPIKEY;
            ht["CT"] = CT;
            ht["HB"] = CaculateHMAC(SingletonManager.MainWindowSingleton.SetMannage.NetHMKEY, message);
            //送出文字列生成
            String param = "";
            foreach (string k in ht.Keys)
            {
                param += String.Format("{0}={1}&", k, HttpUtility.UrlEncode(ht[k].ToString(), Encoding.UTF8));
            }
            byte[] senddata = Encoding.ASCII.GetBytes(param);
            //リクエストを開始する
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(AUTHURL);
            req.Method = "POST";
            req.UserAgent = "";
            req.Headers["X-2ch-UA"] = SingletonManager.MainWindowSingleton.SetMannage.NetX2chUA;
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = senddata.Length;
            //データの書き込み
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(senddata, 0, senddata.Length);
            reqStream.Close();
            //データ取得
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream webs = res.GetResponseStream();
            MemoryStream mems = Ayane.CopyAndClose(webs);
            byte[] bt = new byte[mems.Length];
            mems.Read(bt, 0, bt.Length);
            mems.Close();
            Encoding encode = Ayaka.GetCode(bt);
            String content = encode.GetString(bt);
            AuthKey = content.Split(new Char[] {':'})[1];
            Debug.WriteLine(AuthKey);
        }

        static String getDAT(String server, String boardname, String ID, String SID)
        {
            String message = "/v1/" + server + "/" + boardname + "/" + ID + SID + SingletonManager.MainWindowSingleton.SetMannage.NetAPIKEY;
            String hobo = CaculateHMAC(SingletonManager.MainWindowSingleton.SetMannage.NetHMKEY, message);
            String url = "https://api.2ch.net/v1/" + server + "/" + boardname + "/" + ID;
            //おくるデータ
            Hashtable ht = new Hashtable();
            ht["sid"] = SID;
            ht["hobo"] = hobo;
            ht["appkey"] = SingletonManager.MainWindowSingleton.SetMannage.NetAPIKEY;
            //送出文字列生成
            String param = "";
            foreach (string k in ht.Keys)
            {
                param += String.Format("{0}={1}&", k, HttpUtility.UrlEncode(ht[k].ToString(), Encoding.UTF8));
            }
            byte[] senddata = Encoding.ASCII.GetBytes(param);
            //リクエストする
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.UserAgent = "DOLIB/1.00'";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = senddata.Length;
            //データの書き込み
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(senddata, 0, senddata.Length);
            reqStream.Close();
            //データ取得
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream webs = res.GetResponseStream();
            MemoryStream mems = Ayane.CopyAndClose(webs);
            byte[] bt = new byte[mems.Length];
            mems.Read(bt, 0, bt.Length);
            mems.Close();
            Encoding encode = Ayaka.GetCode(bt);
            String content = encode.GetString(bt);
            return content;
        }
    }
}
