using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Uzuki.Dialogs.MainWindow
{
    public partial class MainWindow : MetroWindow
    {
        List<_2ch.Board> Boardlist = new List<_2ch.Board>();
        List<_2ch.BBSThread> Threadlist = new List<_2ch.BBSThread>();

        //起動時の処理
        void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            //Initialize
            StatusLabel.Content = "板リスト更新中...";
            Thread th = new Thread(getBoardAsync) { IsBackground = true };
            th.Start();
            //てすと
            List<_2ch.Objects.ThreadMesg> list = new List<_2ch.Objects.ThreadMesg>();
            list.Add(new _2ch.Objects.ThreadMesg { Adress = "adress", Message = "てすとてすと", ID = "IDてすと", Name = "あああああああああ" });
            list.Add(new _2ch.Objects.ThreadMesg { Adress = "adress", Message = "てすとてすと", ID = "IDてすと", Name = "あああああああああ" });
            list.Add(new _2ch.Objects.ThreadMesg { Adress = "adress", Message = "てすとてすと", ID = "IDてすと", Name = "あああああああああ" });
            list.Add(new _2ch.Objects.ThreadMesg { Adress = "adress", Message = "てすとてすと", ID = "IDてすと", Name = "あああああああああ" });
            list.Add(new _2ch.Objects.ThreadMesg { Adress = "adress", Message = "てすとてすと", ID = "IDてすと", Name = "あああああああああ" });
            list.Add(new _2ch.Objects.ThreadMesg { Adress = "adress", Message = "http://www.google.co.jp/", ID = "IDてすと", Name = "あああああああああ" });
            ThreadView.ThreadListView.ItemsSource = list;
        }

        // 非同期的に板リストを取得し更新する
        void getBoardAsync()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            String html = wc.DownloadString("http://2ch.sc/bbsmenu.html");
            Boardlist = _2ch.Parser.BBSMenuParser.ParseBBSMenuHTML(html);
            Dispatcher.Invoke(new Action(() =>
            {
                BoardList.BoardListView.ItemsSource = Boardlist;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(BoardList.BoardListView.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("GroupName");
                view.GroupDescriptions.Add(groupDescription);
                StatusLabel.Content = "準備完了";
            }));
        }

        //板が選択された時の処理
        void BoardListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusLabel.Content = "スレッドリスト更新中...";
            GetThreadListAsync gt = new GetThreadListAsync();
            _2ch.Board board = (_2ch.Board)BoardList.BoardListView.SelectedItem;
            gt.URL = board.URL + "/subject.txt";
            gt.Window = this;
            Thread thread = new Thread(gt.getListAsync);
            thread.Start();
        }

        class GetThreadListAsync
        {
            public String URL;
            public MainWindow Window;
            //リスト先駆でスレリストを更新
            public void getListAsync()
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                String text = wc.DownloadString(URL);
                Window.Threadlist = _2ch.Parser.ThreadListParser.ParseThread(text);
                Window.Dispatcher.Invoke(new Action(() =>
                {
                    Window.ThreadList.ThreadListView.ItemsSource = Window.Threadlist;
                    Window.TabCtrl.SelectedIndex = 1;
                    Window.StatusLabel.Content = "準備完了";
                }));
            }
        }
    }
}
