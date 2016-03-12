using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Uzuki._2ch.IDIdentifier
{
    public class IDCache
    {
        public static List<IDItem> IDItemCacheList = new List<IDItem>();
        public class IDItem
        {
            public String ID;
            public String Nickname;
            public SolidColorBrush Brush;
        }
    }
}
