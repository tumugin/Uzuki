using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Uzuki.Controls
{
    /// <summary>
    /// ThreadView.xaml の相互作用ロジック
    /// </summary>
    public partial class ThreadView : UserControl
    {
        public ThreadView()
        {
            InitializeComponent();
            HyperlinkText.OnLinkClick += HyperlinkText_OnLinkClick;
            HyperlinkText.OnResClick += HyperlinkText_OnResClick;
        }

        void HyperlinkText_OnResClick(object sender, EventArgs e)
        {
            //レスクリック時
            //たぶんレスだろ?(しらん)
            Hyperlink link = (Hyperlink)sender;
            Run linkRun = (Run)link.Inlines.FirstInline;
            String linkText = linkRun.Text;
            int index = int.Parse(linkText.Replace(">>", ""));
            ThreadListView.ScrollIntoView(ThreadListView.Items[index - 1]);
        }

        //リンククリック時の動作
        void HyperlinkText_OnLinkClick(object sender, EventArgs e)
        {
            RequestNavigateEventArgs earg = (RequestNavigateEventArgs) e;
            Process.Start(earg.Uri.AbsoluteUri);
        }

    }
}
