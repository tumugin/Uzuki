using MahApps.Metro.Controls;
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

namespace Uzuki.Dialogs.WriteWindow
{
    /// <summary>
    /// WriteWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WriteWindow : MetroWindow
    {
        public CookieContainer cc;
        public String URL;

        public WriteWindow()
        {
            InitializeComponent();
        }

        //書き込みボタン
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "書き込み中...";
            Thread thread = new Thread(WriteThread);
            thread.Start();
        }

        void WriteThread()
        {
            try
            {
                Writer writer = new Writer(URL);
                writer.CookieContainer = cc;
                //writer.CookieContainer.Add(new Cookie("READJS", "off") { Domain = new Uri(writer.PostURL).Host});
                //writer.CookieContainer.Add(new Cookie("MAIL", "") { Domain = new Uri(writer.PostURL).Host });
                //writer.CookieContainer.Add(new Cookie("NAME", "") { Domain = new Uri(writer.PostURL).Host });
                String Name = "", Mail = "", Message = "";
                Dispatcher.Invoke(new Action(() =>
                {
                    Name = NameTextBox.Text;
                    Mail = MailTextBox.Text;
                    Message = MessageTextBox.Text;
                }));
                WriteResponse wr = writer.Write(Name,Mail,Message);
                wr.GetResult();
                if (wr.Result == WriteResponse.WriteResult.True || wr.Result == WriteResponse.WriteResult.False)
                {
                    //成功時
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                    }));
                }
                else
                {
                    //たぶん何かのエラーだ
                    Dispatcher.Invoke(new Action(() =>
                    {
                        StatusLabel.Content = wr.Result;
                    }));
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    StatusLabel.Content = ex.ToString();
                }));
            }
        }
    }
}
