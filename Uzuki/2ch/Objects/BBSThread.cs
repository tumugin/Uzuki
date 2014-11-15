using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzuki._2ch
{
    public class BBSThread
    {
        public String BoardURL { get; set; }
        public String DAT { get; set; }
        public String DATURL { get; set; }
        public String Title { get; set; }
        public int ResCount { get; set; }
        public DateTime createdAt { get; set; }
        public long UnixTime { get; set; }
        public int Number;
    }
}
