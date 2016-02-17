using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzuki._2ch
{
    public class BBSThread : INotifyPropertyChanged
    {
        public String BoardURL { get; set; }
        public String DAT { get; set; }
        public String DATURL { get; set; }
        public String Title { get; set; }
        private int __ResCount;
        public int ResCount {
            get {
                return __ResCount;
            }
            set {
                this.__ResCount = value;
                if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ResCount"));
            }
        }
        public decimal Trend
        {
            get
            {
                decimal diff = Tools.UnixTime.ToUnixTime(DateTime.Now) - UnixTime;
                return ResCount / diff;
            }
        }
        public decimal TrendViewText
        {
            get
            {
                //あまりにも目障りなので桁削ります
                decimal diff = Tools.UnixTime.ToUnixTime(DateTime.Now) - UnixTime;
                return Math.Round((ResCount / diff) * 1000000);
            }
        }
        public DateTime createdAt { get; set; }
        public long UnixTime { get; set; }
        public int ScroolPosItem;
        public int Number;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
