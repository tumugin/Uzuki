using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzuki._2ch.Parser
{
    class BBSMenuParser
    {
        public static List<Board> ParseBBSMenuHTML(String html)
        {
            List<Board> boardList = new List<Board>();
            String[] htmlRow = html.Split(new string[] { "\n",Environment.NewLine }, StringSplitOptions.None);
            String GroupName = null;
            foreach (String str in htmlRow)
            {    
                //グループの始まり
                if (System.Text.RegularExpressions.Regex.IsMatch(str,@"(?i)<BR><BR><B>(.*.)<\/B><BR>"))
                {
                    System.Text.RegularExpressions.MatchCollection mc = System.Text.RegularExpressions.Regex.Matches(str, @"(?i)<BR><BR><B>(.*.)<\/B><BR>");
                    GroupName = mc[0].Groups[1].Value;
                }
                //リンク処理
                else if (System.Text.RegularExpressions.Regex.IsMatch(str, @"<A HREF=(\S+).*.>(.*.)<\/A>") && GroupName != null && GroupName != "ツール類")
                {
                    System.Text.RegularExpressions.MatchCollection mc = System.Text.RegularExpressions.Regex.Matches(str, @"<A HREF=(\S+).*.>(.*.)<\/A>");
                    Board board = new Board { GroupName = GroupName, URL = mc[0].Groups[1].Value, Name = mc[0].Groups[2].Value };
                    boardList.Add(board);
                }
                //空行でクリア
                else if (str == "")
                {
                    GroupName = null;
                }
            }
            return boardList;
        }
    }
}
