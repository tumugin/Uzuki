using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;
using Uzuki._2ch.Write;
using Uzuki.Dialogs.MainWindow;

namespace Uzuki.Dialogs.WriteWindow
{
    /// <summary>
    /// WriteWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WriteWindow : MetroWindow
    {
        public Write2chThreadV2 write2;

        public WriteWindow()
        {
            InitializeComponent();
            //ウィンドウ位置を使いやすい位置に
            this.Top = SingletonManager.MainWindowSingleton.Top;
            this.Left = SingletonManager.MainWindowSingleton.Left;
        }

        //書き込みボタン
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "書き込み中...";
            String name = NameTextBox.Text;
            String mail = MailTextBox.Text;
            String message = MessageTextBox.Text;
            try
            {
                await Task.Run(() => {
                    write2.Write(name, mail, message);
                });
                this.Close();//何もエラーがなければ閉じる
            }
            catch (Write2chThreadV2.Write2chCheckScreenException ex)
            {
                MetroDialogSettings ms = new MetroDialogSettings();
                ms.NegativeButtonText = "キャンセル";
                ms.AffirmativeButtonText = "書き込む";
                ms.ColorScheme = MetroDialogColorScheme.Inverted;
                MessageDialogResult mr = await this.ShowMessageAsync("確認画面", ex.Message, MessageDialogStyle.AffirmativeAndNegative,ms);
                if (mr == MessageDialogResult.Affirmative)
                {
                    Button_Click(null, null);
                }
            }
            catch(Exception ex)
            {
                MetroDialogSettings ms = new MetroDialogSettings();
                ms.NegativeButtonText = "キャンセル";
                ms.AffirmativeButtonText = "再試行";
                ms.ColorScheme = MetroDialogColorScheme.Inverted;
                MessageDialogResult mr = await this.ShowMessageAsync("Error", ex.Message, MessageDialogStyle.AffirmativeAndNegative,ms);
                if (mr == MessageDialogResult.Affirmative)
                {
                    Button_Click(null, null);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            // Display OpenFileDialog by calling ShowDialog method 
            if (dlg.ShowDialog() == true)
            {
                StatusLabel.Content = "Uploading image...";
                new Thread(delegate()
                {
                    try
                    {
                        String url = ImageAPI.imgur.imgur.UploadImage(dlg.FileName);
                        Dispatcher.Invoke(new Action(() => {
                            MessageTextBox.Text += "\r\n" + url;
                            StatusLabel.Content = "画像を投稿しました";
                        }));
                    }
                    catch(Exception ex)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            StatusLabel.Content = ex.Message;
                        }));
                    }
                }).Start();
            }
        }
    }
}
