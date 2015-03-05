using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace Uzuki.Dialogs.MainWindow
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += MainWindow_ContentRendered;
            Closing += MainWindow_Closing;
            BoardList.BoardListView.SelectionChanged += BoardListView_SelectionChanged;
            ThreadList.ThreadListView.SelectionChanged += ThreadListView_SelectionChanged;
            BoardHistoryList.ThreadListView.SelectionChanged += ThreadListView_SelectionChanged;
            ThreadView.ThreadListView.SelectionChanged += ThreadViewListView_SelectionChanged;
            SingletonManager.MainWindowSingleton = this;
            //キレそう
            MenuItem menuItem = (from MenuItem item in ((ContextMenu)ThreadView.ThreadListView.Resources["ItemContextMenu"]).Items where item.Name == "ReplyMenuItem" select item).First();
            MenuItem menuItem2 = (from MenuItem item in ((ContextMenu)ThreadView.ThreadListView.Resources["ItemContextMenu"]).Items where item.Name == "CopyMenuItem" select item).First();
            menuItem.Click += ReplyMenuItem_Click;
            menuItem2.Click += menuItem2_Click;
        }

        void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(BBSThread[ThreadView.ThreadListView.SelectedIndex].Message);
        }

        private void ThreadViewListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var thread = (from _2ch.BBSThread itm in SetMannage.ThreadHistoryList where itm.DATURL == SelectedThread.DATURL select itm).First();
            thread.ScroolPosItem = ((ListView)sender).SelectedIndex;
        }

        void ReplyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WriteWindow.WriteWindow writewindow = new WriteWindow.WriteWindow();
            Uri ur = new Uri(BoardURL);
            writewindow.MessageTextBox.Text = ">>" + (ThreadView.ThreadListView.SelectedIndex + 1);
            writewindow.URL = ur.Scheme + "://" + ur.Host + "/test/read.cgi" + ur.LocalPath + "/" + SelectedThread.UnixTime.ToString() + "/";
            writewindow.cc = SetMannage.Cookie;
            writewindow.Show();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StatusLabel.Content = "板リスト更新中...";
            Thread th = new Thread(getBoardAsync) { IsBackground = true };
            th.Start();
        }

        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (BoardURL == null) return;
            StatusLabel.Content = "スレッドリスト更新中...";
            GetThreadListAsync gt = new GetThreadListAsync();
            gt.URL = BoardURL + "/subject.txt";
            gt.Window = this;
            Thread thread = new Thread(gt.getListAsync);
            thread.Start();
        }

        private void Label_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            var query = from th in (ObservableCollection<_2ch.BBSThread>)ThreadList.ThreadListView.ItemsSource orderby th.createdAt descending select th;
            ObservableCollection<_2ch.BBSThread> list = new ObservableCollection<_2ch.BBSThread>(query.ToList<Uzuki._2ch.BBSThread>());
            ThreadList.ThreadListView.ItemsSource = list;
        }

        private void Label_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            var query = from th in (ObservableCollection<_2ch.BBSThread>)ThreadList.ThreadListView.ItemsSource orderby th.ResCount descending select th;
            ObservableCollection<_2ch.BBSThread> list = new ObservableCollection<_2ch.BBSThread>(query.ToList<Uzuki._2ch.BBSThread>());
            ThreadList.ThreadListView.ItemsSource = list;
        }

        private void Label_MouseDown_4(object sender, MouseButtonEventArgs e)
        {
            var query = from th in (ObservableCollection<_2ch.BBSThread>)ThreadList.ThreadListView.ItemsSource orderby th.Number select th;
            ObservableCollection<_2ch.BBSThread> list = new ObservableCollection<_2ch.BBSThread>(query.ToList<Uzuki._2ch.BBSThread>());
            ThreadList.ThreadListView.ItemsSource = list;
        }

        //上までスクロールする
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ThreadView.ThreadListView.ScrollIntoView(ThreadView.ThreadListView.Items[0]);
            }
            catch (Exception ex)
            {
                //何もしない
            }
        }

        //下までスクロールする
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ThreadView.ThreadListView.ScrollIntoView(ThreadView.ThreadListView.Items[ThreadView.ThreadListView.Items.Count - 1]);
            }
            catch (Exception ex)
            {

            }
        }

        //スレ更新
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (SelectedThread == null) return;
            StatusLabel.Content = "スレッド更新中...";
            GetThreadAsync gt = new GetThreadAsync();
            gt.BThread = SelectedThread;
            gt.setScrollPos = true;
            gt.URL = SelectedThread.DATURL;
            gt.Window = this;
            gt.ThreadName = SelectedThread.Title;
            Thread thread = new Thread(gt.getListAsync);
            thread.Start();
        }

        //検索ボックス
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = from th in Threadlist where th.Title.Contains(SearchTextbox.Text) select th;
            ObservableCollection<_2ch.BBSThread> list = new ObservableCollection<_2ch.BBSThread>(query.ToList<Uzuki._2ch.BBSThread>());
            ThreadList.ThreadListView.ItemsSource = list;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (SelectedThread == null)
            {
                return;
            }
            WriteWindow.WriteWindow writewindow = new WriteWindow.WriteWindow();
            Uri ur = new Uri(SelectedThread.BoardURL);
            writewindow.URL = ur.Scheme + "://" + ur.Host + "/test/read.cgi" + ur.LocalPath + "/" + SelectedThread.UnixTime.ToString() + "/";
            writewindow.cc = SetMannage.Cookie;
            writewindow.Show();
        }

        private void Label_MouseDown_5(object sender, MouseButtonEventArgs e)
        {
            SetMannage.ThreadHistoryList.Clear();
        }

        private void Label_MouseDown_6(object sender, MouseButtonEventArgs e)
        {
            if(BoardHistoryList.ThreadListView.SelectedIndex != -1) SetMannage.ThreadHistoryList.RemoveAt(BoardHistoryList.ThreadListView.SelectedIndex);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Dialogs.SettingDialog dialog = new SettingDialog();
            dialog.Show();
        }

        private void Label_MouseDown_7(object sender, MouseButtonEventArgs e)
        {
            if (BoardURL == null) return;
            WriteWindow.NewThreadWindow window = new WriteWindow.NewThreadWindow();
            Uri ur = new Uri(BoardURL);
            window.URL = ur.Scheme + "://" + ur.Host + "/test/read.cgi" + ur.LocalPath + "/";
            window.cc = SetMannage.Cookie;
            window.Show();
        }
    }
}
