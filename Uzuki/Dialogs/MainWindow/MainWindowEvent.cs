using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Uzuki.Controls;
using Uzuki.Network;

namespace Uzuki.Dialogs.MainWindow
{
    public partial class MainWindow : MetroWindow
    {
        ObservableCollection<_2ch.Board> Boardlist = new ObservableCollection<_2ch.Board>();
        ObservableCollection<_2ch.BBSThread> Threadlist = new ObservableCollection<_2ch.BBSThread>();
        ObservableCollection<_2ch.Objects.ThreadMesg> BBSThread = new ObservableCollection<_2ch.Objects.ThreadMesg>();
        public String BoardURL;
        public _2ch.BBSThread SelectedThread;
        public Settings.SettingManager SetMannage;

        //起動時の処理
        void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            //Load setting
            SetMannage = Settings.SettingManager.LoadSettings();
            //Binding
            TreeViewRadioButton.SetBinding(RadioButton.IsCheckedProperty, new Binding("EnableTreeView") { Source = SetMannage });
            ResViewRadioButton.SetBinding(RadioButton.IsCheckedProperty, new Binding("EnableResView") { Source = SetMannage });
            //TESTING THEME
            if (SetMannage.UseBlackTheme) {
                ThemeColor.MkThemeSelector.AddGlobalThemeDictonary(new Uri("/MahApps.Metro;component/Styles/Accents/BaseDark.xaml", UriKind.Relative));
                ThemeColor.MkThemeSelector.AddGlobalThemeDictonary(new Uri("/Uzuki;component/ThemeColor/DarkTheme.xaml", UriKind.Relative));
            }
            //Initialize
            StatusLabel.Content = "板リスト更新中...";
            Thread th = new Thread(getBoardAsync) { IsBackground = true };
            th.Start();

            BoardHistoryList.ThreadListView.ItemsSource = SetMannage.ThreadHistoryList;

            //アプデチェック
            UpdateChecker.UpdateNotify.checkUpdate();
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
            try
            {
                String html = Ayane.getHttp(SetMannage.BBSMenuPath);
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
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    StatusLabel.Content = ex.Message;
                }));
            }
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
            //スレリストを更新
            public void getListAsync()
            {
                try
                {
                    String text = Ayane.getHttp(URL);
                    Window.Threadlist = new ObservableCollection<_2ch.BBSThread>(_2ch.Parser.ThreadListParser.ParseThread(text));
                    foreach (_2ch.BBSThread th in Window.Threadlist)
                    {
                        th.DATURL = Window.BoardURL + "/dat/" + th.DAT;
                        th.BoardURL = Window.BoardURL;
                    }
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        Window.ThreadList.ThreadListView.ItemsSource = Window.Threadlist;
                        Window.ThreadList.ThreadListView.ScrollIntoView(Window.ThreadList.ThreadListView.Items[0]);
                        Window.TabCtrl.SelectedIndex = 1;
                        Window.StatusLabel.Content = "準備完了";
                    }));
                }
                catch (Exception ex)
                {
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        Window.StatusLabel.Content = ex.Message;
                    }));
                }
            }
        }

        //スレッド選択
        void ThreadListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (ThreadList.ThreadListView.SelectedItem == null) return;
            StatusLabel.Content = "スレッド更新中...";
            //親を取得
            ListView lview = (ListView)sender;
            _2ch.BBSThread th = (_2ch.BBSThread) lview.SelectedItem;
            if (th == null) return; //バグ防止
            //ダブリが無いか確認してから履歴に追加
            if ((from itm in SetMannage.ThreadHistoryList where itm.DATURL == th.DATURL select itm).Count() == 0 )
            {
                SetMannage.ThreadHistoryList.Insert(0,th);
            }
            else
            {
                //ダブリがある時は上に持っていく
                var item = from itm in SetMannage.ThreadHistoryList where itm.DATURL == th.DATURL select itm;
                var clone = item.First();
                SetMannage.ThreadHistoryList.Remove(item.First());
                SetMannage.ThreadHistoryList.Insert(0,clone);
            }
            //BoardURLも更新しておく
            BoardURL = th.BoardURL;
            //クソッタレ
            SelectedThread = th;
            //update
            updateThreadView(th);
        }

        async void updateThreadView(_2ch.BBSThread th,bool updateScroll = true)
        {
            //ここから別スレッドで
            ObservableCollection<_2ch.Objects.ThreadMesg> tlist = null;
            try
            {
                await Task.Run(() =>
                {
                    String text = Ayane.getDAT(th.DATURL);
                    String decodetext = HttpUtility.HtmlDecode(text);
                    tlist = new ObservableCollection<_2ch.Objects.ThreadMesg>(_2ch.Parser.ThreadParser.ParseThread(decodetext));
                    //ソートの有無
                    if(SetMannage.EnableTreeView) _2ch.Parser.ThreadParser.SortByRes(ref tlist);
                    //履歴アイテムの情報を更新
                    var historyitem = from itm in SetMannage.ThreadHistoryList where itm.DATURL == th.DATURL select itm;
                    historyitem.First().ResCount = tlist.Count();
                });
                BackgroundLabel.Text = th.Title;
                BBSThread = tlist;
                ThreadView.ThreadListView.ItemsSource = BBSThread;
                WPFUtil.DoEvents(); //強制再描画
                if(updateScroll) try { ThreadView.ThreadListView.ScrollIntoView(ThreadView.ThreadListView.Items[th.ScroolPosItem]); } catch { }
                StatusLabel.Content = "準備完了";
            }
            catch (Exception ex)
            {
                StatusLabel.Content = ex.Message;
            }
        }
    }
}
