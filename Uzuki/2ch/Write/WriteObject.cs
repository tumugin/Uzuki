using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Uzuki._2ch.Write
{
    public class Writer
    {
        /// <summary>
		/// Postする場所のURL
		/// bbs.cgiのURL
		/// </summary>
		public string PostURL = "";
		/// <summary>
		/// RefererのURL
		/// </summary>
		public string RefererURL = "";
		/// <summary>
		/// 板の名前
		/// </summary>
		/// <example>new4vip</example>
		public string BoardKey = "";
		/// <summary>
		/// 書き込むスレッドのキー
		/// </summary>
		public string ThreadKey = "";
		/// <summary>
		/// 書き込む時に使用するCookieContainerです
		/// </summary>
		public CookieContainer CookieContainer;
		/// <summary>
		/// User-Agentの値です
		/// </summary>
        protected static string UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.77 Safari/537.36";
		/// <summary>
		/// 書き込む際の文字エンコードです
		/// </summary>
		public static Encoding WriteEncoding = Encoding.GetEncoding("Shift-JIS");
		/// <summary>
		/// 書き込む際のsumbitの値
		/// </summary>
		public static string WriteSumbit = "書き込む";
		/// <summary>
		/// スレッド作成の際のsumbitの値
		/// </summary>
		public string CreateThreadSumbit = "新規スレッドを作成";
		/// <summary>
		/// 空のクラスを作成します
		/// </summary>
		public Writer()
		{
		}
		/// <summary>
		/// スレッドのURLまたは板のURLから作成します
		/// </summary>
		/// <param name="ThreadURL">解析するURL</param>
		public Writer(string ThreadURL)
		{
			this.FromURL(ThreadURL);
		}
		/// <summary>
		/// URLを指定してPostURL、BoardKey、ThreadKeyを自動取得します
		/// </summary>
		/// <param name="ThreadURL">解析するURL</param>
		public void FromURL(string ThreadURL)
		{
			this.BoardKey = Regex.Match(ThreadURL, "read\\.cgi/([^/]*)").Groups[1].Value;
			this.ThreadKey = Regex.Match(ThreadURL, "read\\.cgi/[^/]*/([^/]*)").Groups[1].Value;
			string value = Regex.Match(ThreadURL, "http://[^/]*/").Value;
			this.RefererURL = value + this.ThreadKey;
			this.PostURL = value + "test/bbs.cgi?guid=ON";
		}
		/// <summary>
		/// 名前、メール、本文を指定して書き込みます
		/// </summary>
		/// <param name="Name">名前</param>
		/// <param name="Mail">メール</param>
		/// <param name="Message">本文</param>
		/// <returns>結果を解析するクラス</returns>
		public WriteResponse Write(string Name, string Mail, string Message)
		{
			return this.Request(this.MakeHttpWebRequest(), this.MakeWriteParam(Name, Mail, Message));
		}
		/// <summary>
		/// 名前、メール、本文、スレッド名を指定してスレッドを作ります
		/// </summary>
		/// <param name="Name">名前</param>
		/// <param name="Mail">メール</param>
		/// <param name="Message">本文</param>
		/// <param name="Subject">スレッド名</param>
		/// <returns>結果を解析するクラス</returns>
		public WriteResponse CreateThread(string Name, string Mail, string Message, string Subject)
		{
			return this.Request(this.MakeHttpWebRequest(), this.MakeCreateThreadParam(Name, Mail, Message, Subject));
		}
		/// <summary>
		/// HttpWebRequestとパラメーターを指定してリクエストを送信します
		/// </summary>
		protected WriteResponse Request(HttpWebRequest req, byte[] data)
		{
			req.ContentLength = (long)data.Length;
			Stream requestStream = req.GetRequestStream();
			requestStream.Write(data, 0, data.Length);
			requestStream.Close();
			return new WriteResponse(req);
		}
		/// <summary>
		/// 書き込みに使うHttpWebRequestを作成します
		/// </summary>
		/// <returns></returns>
		protected HttpWebRequest MakeHttpWebRequest()
		{
			HttpWebRequest httpWebRequest = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(this.PostURL);
			}
			catch (UriFormatException)
			{
				throw new Exception("PostURLが無効です");
			}
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.CookieContainer = this.CookieContainer;
			httpWebRequest.Referer = this.RefererURL;
			httpWebRequest.UserAgent = Writer.UserAgent;
			httpWebRequest.Proxy = null;
			return httpWebRequest;
		}
		/// <summary>
		/// 書き込む際bbs.cgiに渡す引数を作成
		/// </summary>
		protected byte[] MakeWriteParam(string Name, string Mail, string Message)
		{
			return Encoding.ASCII.GetBytes(string.Concat(new string[]
			{
				"bbs=",
				this.BoardKey,
				"&key=",
				this.ThreadKey,
				"&time=1&submit=",
				HttpUtility.UrlEncode(Writer.WriteSumbit, Writer.WriteEncoding),
				"&FROM=",
				HttpUtility.UrlEncode(Name, Writer.WriteEncoding),
				"&mail=",
				HttpUtility.UrlEncode(Mail, Writer.WriteEncoding),
				"&MESSAGE=",
				HttpUtility.UrlEncode(Message, Writer.WriteEncoding)
			}));
		}
		/// <summary>
		/// スレッド作成の際bbs.cgiに渡す引数を作成
		/// </summary>
		protected byte[] MakeCreateThreadParam(string Name, string Mail, string Message, string Subject)
		{
			return Encoding.ASCII.GetBytes(string.Concat(new string[]
			{
				"bbs=",
				this.BoardKey,
				"&time=1&submit=",
				HttpUtility.UrlEncode(this.CreateThreadSumbit, Writer.WriteEncoding),
				"&subject=",
				HttpUtility.UrlEncode(Subject, Writer.WriteEncoding),
				"&FROM=",
				HttpUtility.UrlEncode(Name, Writer.WriteEncoding),
				"&mail=",
				HttpUtility.UrlEncode(Mail, Writer.WriteEncoding),
				"&MESSAGE=",
				HttpUtility.UrlEncode(Message, Writer.WriteEncoding)
			}));
		}
    }

    /// <summary>
    /// 書き込んだ結果を取得するクラスです。
    /// GetResultを実行すると結果を取得できます。
    /// </summary>
    public class WriteResponse
    {
        /// <summary>
        /// Requestが成されたHttpWebRequestです
        /// </summary>
        protected HttpWebRequest req;
        /// <summary>
        /// 結果を取得しているかどうかを示します
        /// </summary>
        public bool isGet;
        /// <summary>
        /// 結果のHTMLのタイトルが入ります
        /// </summary>
        public string Title = "";
        /// <summary>
        /// 結果のHTMLの2chタグが入ります
        /// </summary>
        public string Tag2ch = "";
        /// <summary>
        /// 結果のHTMLのソースが入ります
        /// </summary>
        public string Html = "";
        /// <summary>
        /// 書き込んだ結果が入ります
        /// </summary>
        public WriteResult Result = WriteResult.None;
        /// <summary>
        /// HTMLの取得に使うエンコード
        /// </summary>
        public static Encoding HTMLEncoding = Encoding.GetEncoding("Shift-JIS");
        /// <summary>
        /// 書き込みに使用したHttpWebRequestから作成します
        /// </summary>
        /// <param name="req">書き込みに使用したHttpWebRequest</param>
        public WriteResponse(HttpWebRequest req)
        {
            this.req = req;
        }
        /// <summary>
        /// タイトルの文字列を取得します
        /// </summary>
        protected string GetHTMLTitle()
        {
            return Regex.Match(this.Html, "<title\\s?[^>]*>(.*)</title>", RegexOptions.IgnoreCase).Groups[1].Value;
        }
        /// <summary>
        /// 2chタグの内容(\!-- 2ch_X:*** --&gt;の***)を取得します
        /// </summary>
        /// <returns></returns>
        protected string GetHTML2chTag()
        {
            return Regex.Match(this.Html, "<\\!--\\s*2ch_X:(\\w*)\\s*-->").Groups[1].Value;
        }
        /// <summary>
        /// 結果を取得します
        /// 多少遅延が発生します
        /// TODO:非同期対応
        /// </summary>
        public void GetResult()
        {
            if (this.isGet)
            {
                return;
            }
            this.Html = this.GetResponseHtml();
            this.Title = this.GetHTMLTitle();
            this.Tag2ch = this.GetHTML2chTag();
            this.Result = WriteResponse.GetWriteResult(this.Title, this.Tag2ch);
            this.isGet = true;
        }
        /// <summary>
        /// タイトルと2chタグの内容から書き込み結果を判定します
        /// </summary>
        /// <param name="title">ページのタイトル</param>
        /// <param name="tag2ch">2chタグの内容</param>
        protected static WriteResult GetWriteResult(string title, string tag2ch)
        {
            string a;
            if ((a = tag2ch.ToLower()) != null)
            {
                if (a == "true")
                {
                    return WriteResult.True;
                }
                if (a == "false")
                {
                    return WriteResult.False;
                }
                if (a == "error")
                {
                    return WriteResult.Error;
                }
                if (a == "check")
                {
                    return WriteResult.Check;
                }
                if (a == "cookie")
                {
                    return WriteResult.Cookie;
                }
            }
            if (title.Contains("書きこみました"))
            {
                return WriteResult.True;
            }
            return WriteResult.Error;
        }
        /// <summary>
        /// HttpWebRequestから返されるHTMLを取得します
        /// 取得できるのは一度のみ
        /// </summary>
        /// <returns></returns>
        protected string GetResponseHtml()
        {
            HttpWebResponse httpWebResponse = (HttpWebResponse)this.req.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, WriteResponse.HTMLEncoding);
            string result = streamReader.ReadToEnd();
            responseStream.Close();
            streamReader.Close();
            httpWebResponse.Close();
            return result;
        }
        /// <summary>
        /// 書き込んだ結果を示す定数です
        /// </summary>
        public enum WriteResult
        {
            /// <summary>
            /// 書き込みに成功しました
            /// </summary>
            True,
            /// <summary>
            /// 書き込みに成功しましたが注意書きが付きました
            /// </summary>
            False,
            /// <summary>
            /// 書き込みになんらかのエラーが出て失敗しました
            /// </summary>
            Error,
            /// <summary>
            /// チェック画面になり書き込めませんでした
            /// </summary>
            Check,
            /// <summary>
            /// クッキー設定画面になり書き込めませんでした
            /// </summary>
            Cookie,
            /// <summary>
            /// 結果は取得されていません
            /// </summary>
            None
        }
    }
}
