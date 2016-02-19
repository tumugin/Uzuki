using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Uzuki._2ch.IDIdentifier
{
    public class VoiceActor
    {
        public static string[] Names = {
            "野中",
            "堀江",
            "内田",
            "伊藤",
            "早見",
            "種田",
            "水瀬",
            "村川",
            "楠田",
            "徳井",
            "日笠",
            "三森",
            "田村",
            "佐倉",
            "小林",
            "戸松",
            "南條",
            "小倉",
            "沢城",
            "小松",
            "金元",
            "釘宮",
            "豊崎",
            "阿澄",
            "井口",
            "高垣",
            "水樹",
            "平野",
            "花澤",
            "井上",
            "白石",
            "坂本",
            "斎藤",
            "東山",
            "久野",
            "雨宮",
            "名塚",
            "中原",
            "大西",
            "大原",
            "西",
            "飯田",
            "久保",
            "福原",
            "上坂",
            "後藤",
            "生天目",
            "石原",
            "野水",
            "中島",
            "大久保",
            "五十嵐",
            "能登",
            "竹達",
            "悠木",
            "加藤",
            "明坂",
            "茅野",
            "遠藤",
            "矢作",
            "たかはし",
            "田中",
            "内山",
            "佐藤",
            "喜多村",
            "小岩井",
            "木戸",
            "沼倉",
            "清水",
            "福圓",
            "津田",
            "大坪",
            "加隈",
            "三澤",
            "植田",
            "赤崎",
            "小澤",
            "高橋",
            "諸星",
            "新田",
            "高森",
            "民安",
            "相坂",
            "田辺",
            "山崎",
            "黒沢",
            "夏川",
            "長縄",
            "吉岡",
            "茜屋",
            "麻倉",
            "井澤",
            "久保田",
            "芹澤",
            "日高",
            "稲川",
            "今村",
            "上田",
            "小川",
            "瀬戸",
            "田所",
            "松田",
            "朝井",
            "角元",
            "永野",
            "橋本",
            "奥野",
            "戸田",
            "青木",
            "石川",
            "原",
            "高部",
            "寺崎",
            "秦",
            "鳴門",
            "牧野",
            "新谷",
            "丹下",
            "末来"};

        public class NameItem
        {
            public string Name;
            public bool isUsed = false;
            public SolidColorBrush SBrush;
        }

        public static List<NameItem> getNameList()
        {
            List<NameItem> list = new List<NameItem>();
            Random rand = new Random();
            foreach(string s in Names)
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255,(byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
                //Freezeさせないとエラーになる
                brush.Freeze();
                list.Add(new NameItem { Name = s ,SBrush = brush});
            }
            return list;
        }
    }
}
