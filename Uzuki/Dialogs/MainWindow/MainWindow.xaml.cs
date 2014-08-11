using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            BoardList.BoardListView.SelectionChanged += BoardListView_SelectionChanged;
            ThreadList.ThreadListView.SelectionChanged += ThreadListView_SelectionChanged;
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
            var query = from th in Threadlist orderby th.createdAt descending select th;
            Threadlist = new ObservableCollection<_2ch.BBSThread>(query.ToList<Uzuki._2ch.BBSThread>());
            ThreadList.ThreadListView.ItemsSource = Threadlist;
        }

        private void Label_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            var query = from th in Threadlist orderby th.ResCount descending select th;
            Threadlist = new ObservableCollection<_2ch.BBSThread>(query.ToList<Uzuki._2ch.BBSThread>());
            ThreadList.ThreadListView.ItemsSource = Threadlist;
        }

        private void Label_MouseDown_4(object sender, MouseButtonEventArgs e)
        {
            var query = from th in Threadlist orderby th.Number select th;
            Threadlist = new ObservableCollection<_2ch.BBSThread>(query.ToList<Uzuki._2ch.BBSThread>());
            ThreadList.ThreadListView.ItemsSource = Threadlist;
        }
    }
}
