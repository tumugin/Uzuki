using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uzuki;
using Uzuki._2ch.Write;
using System.Net;
using System.Diagnostics;

namespace UzukiTest
{
    [TestClass]
    public class WriterTester
    {
        [TestMethod]
        public void WriteTest()
        {
            CookieContainer ck = new CookieContainer();
            Write2chThreadV2 write = new Write2chThreadV2("ghard", "1455342643", "wktk.2ch.net", ck);
            try
            {
                write.Write("","sage","てすと");
            }
            catch (Exception ex)
            {
                //書き込みエラーメッセージは例外をスローする
                Debug.WriteLine(ex.Message);
            }
            
            //2回目送信すると認証を通過できる
            write.Write("", "sage", "てすと");
        }

        [TestMethod]
        public void WriteOldTest()
        {
            CookieContainer ck = new CookieContainer();
            Writer writer = new Writer("http://sweet.2ch.sc/test/read.cgi/kawaii/1438245456/");
            WriteResponse wr =  writer.Write("", "sage", "てすとてすと");
            wr.GetResult();
            Debug.WriteLine(wr.Result);
            WriteResponse wr2 = writer.Write("", "sage", "てすとてすと");
            wr2.GetResult();
            Debug.WriteLine(wr2.Result);
        }
    }
}
