﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Uzuki._2ch.Objects;
using Uzuki.Dialogs.SubDialogs;

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
            try
            {
                Hyperlink link = (Hyperlink)sender;
                Run linkRun = (Run)link.Inlines.FirstInline;
                String linkText = linkRun.Text;
                int index = int.Parse(linkText.Replace(">>", ""));
                Dialogs.SubDialogs.ResDialog resdiag = new Dialogs.SubDialogs.ResDialog();
                var list = Convert<ThreadMesg>(ThreadListView.Items);
                var query = from itm in list where itm.Number == index select itm;
                if (query.Count() == 0) return;
                resdiag.Resdata = query.First();
                resdiag.Resdata.Count = index;
                resdiag.DataContext = resdiag.Resdata;
                resdiag.Show();
                //ThreadListView.ScrollIntoView(ThreadListView.Items[index - 1]);
            }
            catch (Exception ignore) { }
        }

        //リンククリック時の動作
        void HyperlinkText_OnLinkClick(object sender, EventArgs e)
        {
            RequestNavigateEventArgs earg = (RequestNavigateEventArgs)e;
            String URL = earg.Uri.AbsoluteUri;
            //ttpのとき
            if (earg.Uri.Scheme == "ttp")
            {
                URL = "h" + URL;
            }
            //imgur
            if (Regex.IsMatch(URL,"^.*.(jpg|gif|bmp|png|jpeg).*$"))
            {
                ImagePreviewDialog diag = new ImagePreviewDialog();
                diag.URL = URL;
                diag.Show();
            }else{
                Process.Start(URL);
            }
        }

        //observablecollectionを無理矢理生成する
        //http://stackoverflow.com/questions/3559821/how-to-convert-ienumerable-to-observablecollection

        public ObservableCollection<object> Convert(IEnumerable original)
        {
            return new ObservableCollection<object>(original.Cast<object>());
        }
        public ObservableCollection<T> Convert<T>(IEnumerable<T> original)
        {
            return new ObservableCollection<T>(original);
        }
        public ObservableCollection<T> Convert<T>(IEnumerable original)
        {
            return new ObservableCollection<T>(original.Cast<T>());
        }
    }
}
