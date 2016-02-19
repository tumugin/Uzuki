using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uzuki._2ch.Objects;

namespace Uzuki._2ch.Parser
{
    class ThreadParser
    {
        public static List<_2ch.Objects.ThreadMesg> ParseThread(String text)
        {
            String[] Row = text.Split(new string[] { "\n", Environment.NewLine }, StringSplitOptions.None);
            List<_2ch.Objects.ThreadMesg> list = new List<Objects.ThreadMesg>();
            int count = 1;
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
                    mesg.Number = count;
                    count++;
                    MatchCollection mc = Regex.Matches(mesg.ID, "ID:(.*.)");
                    if(mc.Count != 0) mesg.AuthorID = mc[0].Groups[0].Value;
                    //HTMLタグとかを取り除く
                    mesg.Message = System.Web.HttpUtility.HtmlDecode(lineSplit[3]);
                    mesg.Message = mesg.Message.Replace("<br>", Environment.NewLine);
                    mesg.Message = Regex.Replace(mesg.Message, "<.*?>", "");
                    mesg.Name = Regex.Replace(mesg.Name, "<.*?>", "");
                    if (Regex.IsMatch(mesg.Message, @">>([0-9]+)"))
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
            //自動あだ名機能
            Random Rand = new Random();
            List<IDIdentifier.VoiceActor.NameItem> voiceactor = IDIdentifier.VoiceActor.getNameList();
            foreach (ThreadMesg mes in list)
            {
                var sameid = from itm in list where itm.AuthorID == mes.AuthorID && itm.Nickname == "" select itm;
                //値を変更するとリストのカウントも変わってしまうので一時的に別変数に退避させる
                int sameidCount = sameid.Count();
                //一つしか無いならor全くない(処理済み)は処理しない
                if (sameid.Count() <= 1) continue;
                int adcount = 1;
                //ランダムな名前を取得する
                var randomnames = from itm in voiceactor where itm.isUsed == false select itm;
                //名前を使い切ったなら諦める
                if (randomnames.Count() == 0) break;
                IDIdentifier.VoiceActor.NameItem random = randomnames.ElementAt(Rand.Next(randomnames.Count()));
                random.isUsed = true;
                foreach (ThreadMesg th in sameid)
                {
                    th.NicknameCount = "(" + adcount.ToString() + "/" + sameidCount.ToString() + ")";
                    th.Nickname = random.Name;
                    th.SBrush = random.SBrush;
                    adcount++;
                }
            }
            return list;
        }

        public static void SortByRes(ref ObservableCollection<ThreadMesg> list)
        {
            //リストの順を元通りに直す
            list = new ObservableCollection<ThreadMesg>(from itm in list orderby itm.Number ascending select itm);
            //レスを抽出
            var query = from th in list where th.isReply select th;
            //コピーを作っておく(でないと変更時にエラーになる)
            query = new ObservableCollection<ThreadMesg>(query);
            foreach (ThreadMesg mesg in query)
            {
                //該当レス番号の下に移動させる
                MatchCollection mc = Regex.Matches(mesg.Message, @">>([0-9]+)");
                //TODO: 複数ある場合もあるけどとりあえず一つ目のやつで判定する(そのうち直す)
                int id = int.Parse(mc[0].Groups[1].Value);
                if(id <= list.Count && mc.Count == 1)
                {
                    ThreadMesg m = (from itm in list where itm.Number == id select itm).First();
                    list.Move(list.IndexOf(mesg), list.IndexOf(m) + 1);
                }
            }
        }

        public static void ResetSort(ref ObservableCollection<ThreadMesg> list)
        {
            //リストの順を元通りに直す
            list = new ObservableCollection<ThreadMesg>(from itm in list orderby itm.Number ascending select itm);
        }
    }
}
