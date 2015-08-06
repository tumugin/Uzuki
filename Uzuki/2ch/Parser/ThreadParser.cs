using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Uzuki._2ch.Parser
{
    class ThreadParser
    {
        public static List<_2ch.Objects.ThreadMesg> ParseThread(String text)
        {
            String[] Row = text.Split(new string[] { "\n", Environment.NewLine }, StringSplitOptions.None);
            List<_2ch.Objects.ThreadMesg> list = new List<Objects.ThreadMesg>();
            foreach (String line in Row)
            {
                try
                {
                    if (line == "") break;
                    Objects.ThreadMesg mesg = new Objects.ThreadMesg();
                    String[] lineSplit = line.Split(new String[] { "<>" }, StringSplitOptions.None);
                    mesg.Name = lineSplit[0];
                    mesg.Adress = lineSplit[1];
                    mesg.ID = lineSplit[2];
                    MatchCollection mc = Regex.Matches(mesg.ID, "ID:(.*.)");
                    if(mc.Count != 0) mesg.AuthorID = mc[0].Groups[0].Value;
                    //HTMLタグとかを取り除く
                    mesg.Message = System.Web.HttpUtility.HtmlDecode(lineSplit[3]);
                    mesg.Message = mesg.Message.Replace("<br>", Environment.NewLine);
                    mesg.Message = Regex.Replace(mesg.Message, "<.*?>", "");
                    mesg.Name = Regex.Replace(mesg.Name, "<.*?>", "");
                    if (Regex.IsMatch(mesg.Message, @">>(\d+)"))
                    {
                        //これはレスだ
                        mesg.isReply = true;
                    }
                    list.Add(mesg);
                }
                catch (Exception aznyan)
                {
                    Debug.WriteLine(aznyan.ToString());
                }
            }
            //スレ主を取得する
            String author = list[0].AuthorID;
            var query = from th in list where th.AuthorID == author select th;
            //2chがバグっててIDの記録に失敗している時は、とりあえずスレ主判定とかを無くす
            if (author != null && author != "ID:???")
            {
                foreach (Objects.ThreadMesg mesg in query)
                {
                    mesg.isThreadNushi = true;
                }
            }
            return list;
        }
    }
}
