using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzuki._2ch.Objects
{
    public class ThreadHistoryObj
    {
        public String DatURL { get; set; }
        public int ReadPosition { get; set; }
        public String Title { get; set; }
        public List<String> ThreadNushiList { get; set; }
    }
}
