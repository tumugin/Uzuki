using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uzuki._2ch.IDIdentifier;

namespace UzukiTest
{
    [TestClass]
    public class NameIdentifierTester
    {
        [TestMethod]
        public void GetDuplicate()
        {
            foreach(string s in VoiceActor.Names)
            {
                var query = from item in VoiceActor.Names where item == s select item;
                if(query.Count() > 1)
                {
                    Debug.WriteLine("重複:" + s);
                }
            }
        }
    }
}
