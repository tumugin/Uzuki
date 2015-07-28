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
        public DateTime createdAt { get; set; }
        public long UnixTime { get; set; }
        public int ScroolPosItem;
        public int Number;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
