using System;
using System.Collections.Generic;
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
                if (line == "") break;
                Objects.ThreadMesg mesg = new Objects.ThreadMesg();
                String[] lineSplit = line.Split(new String[] { "<>" }, StringSplitOptions.None);
                mesg.Name = lineSplit[0];
                mesg.Adress = lineSplit[1];
                mesg.ID = lineSplit[2];
                //HTMLタグとかを取り除く
                mesg.Message = System.Web.HttpUtility.HtmlDecode(lineSplit[3]);
                mesg.Message = mesg.Message.Replace("<br>", Environment.NewLine);
                mesg.Message = Regex.Replace(mesg.Message, "<.*?>", "");
                if (Regex.IsMatch(mesg.Message, @">>(\d+)"))
                {
                    //これはレスだ
                    mesg.isReply = true;
                }
                list.Add(mesg);
            }
            return list;
        }
    }
}
