using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzuki._2ch.Objects
{
    class ThreadMesg
    {
        public String Name { get; set; }
        public String ID { get; set; }
        public String Adress { get; set; }
        public String Message { get; set; }
        public bool isThreadNushi { get; set; }
        public bool isReply { get; set; }
    }
}
