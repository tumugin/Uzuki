using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Uzuki._2ch.Objects
{
    public class ThreadMesg
    {
        public int Count { get; set; }
        public int Number { get; set; }
        public String Name { get; set; }
        public String ID { get; set; }
        public String Adress { get; set; }
        public String Message { get; set; }
        public String AuthorID { get; set; }
        public bool isThreadNushi { get; set; }
        public bool isReply { get; set; }
        public bool isInNG { get; set; } = false;
        public String Nickname { get; set; } = "";
        public String NicknameCount { get; set; } = "";
        public SolidColorBrush SBrush { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
        
        public ThreadMesg()
        {
            //Freezeさせないとエラーになる
            SBrush.Freeze();
        }
    }
}
