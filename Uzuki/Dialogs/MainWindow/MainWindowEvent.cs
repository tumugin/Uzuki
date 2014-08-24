using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace Uzuki.Dialogs.MainWindow
{
    public partial class MainWindow : MetroWindow
    {
        ObservableCollection<_2ch.Board> Boardlist = new ObservableCollection<_2ch.Board>();
        ObservableCollection<_2ch.BBSThread> Threadlist = new ObservableCollection<_2ch.BBSThread>();
        ObservableCollection<_2ch.Objects.ThreadMesg> BBSThread = new ObservableCollection<_2ch.Objects.ThreadMesg>();
        String BoardURL;
        _2ch.BBSThread SelectedThread;
        Settings.SettingManager SetMannage;

        //起動時の処理
        void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            //Load setting
            SetMannage = Settings.SettingManager.LoadSettings();
            //Initialize
            StatusLabel.Content = "板リスト更新中...";
            Thread th = new Thread(getBoardAsync) { IsBackground = true };
            th.Start();
        }

        //終了時の処理
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //設定を保存するよ
            SetMannage.SaveSettings();
        }

        // 非同期的に板リストを取得し更新する
        void getBoardAsync()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            String html = wc.DownloadString(SetMannage.BBSMenuPath);
            Boardlist = new ObservableCollection<_2ch.Board>(_2ch.Parser.BBSMenuParser.ParseBBSMenuHTML(html));
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
            BoardURL = board.URL;
            gt.Window = this;
            Thread thread = new Thread(gt.getListAsync);
            thread.Start();
        }

        //非同期的にスレ一覧を取ってくるぞ
        class GetThreadListAsync
        {
            public String URL;
            public MainWindow Window;
            //リスト先駆でスレリストを更新
            public void getListAsync()
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                String text = wc.DownloadString(URL);
                Window.Threadlist = new ObservableCollection<_2ch.BBSThread>(_2ch.Parser.ThreadListParser.ParseThread(text));
                Window.Dispatcher.Invoke(new Action(() =>
                {
                    Window.ThreadList.ThreadListView.ItemsSource = Window.Threadlist;
                    Window.ThreadList.ThreadListView.ScrollIntoView(Window.ThreadList.ThreadListView.Items[0]);
                    Window.TabCtrl.SelectedIndex = 1;
                    Window.StatusLabel.Content = "準備完了";
                }));
            }
        }

        //スレッド選択
        void ThreadListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThreadList.ThreadListView.SelectedItem == null) return;
            StatusLabel.Content = "スレッド更新中...";
            GetThreadAsync gt = new GetThreadAsync();
            _2ch.BBSThread th = (_2ch.BBSThread) ThreadList.ThreadListView.SelectedItem;
            SelectedThread = th;
            gt.URL = BoardURL + "/dat/" + th.DAT;
            gt.Window = this;
            gt.ThreadName = th.Title;
            Thread thread = new Thread(gt.getListAsync);
            thread.Start();
        }

        //非同期的にスレ一覧を取ってくるぞ
        class GetThreadAsync
        {
            public String URL;
            public MainWindow Window;
            public String ThreadName;
            //リスト先駆でスレリストを更新
            public void getListAsync()
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                String text = wc.DownloadString(URL);
                Window.BBSThread = new ObservableCollection<_2ch.Objects.ThreadMesg>(_2ch.Parser.ThreadParser.ParseThread(text));
                Window.Dispatcher.Invoke(new Action(() =>
                {
                    Window.BackgroundLabel.Text = ThreadName;
                    Window.ThreadView.ThreadListView.ItemsSource = Window.BBSThread;
                    Window.ThreadView.ThreadListView.ScrollIntoView(Window.ThreadView.ThreadListView.Items[0]);
                    Window.StatusLabel.Content = "準備完了";
                }));
            }
        }
    }
}
