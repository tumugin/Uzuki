using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using MahApps.Metro.Controls.Dialogs;
using Uzuki.Dialogs.MainWindow;

namespace Uzuki.Dialogs.SubDialogs
{
    /// <summary>
    /// ImagePreviewDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ImagePreviewDialog : MetroWindow
    {
        public String URL;

        public ImagePreviewDialog()
        {
            InitializeComponent();
            this.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Inverted;
            //ウィンドウ位置を使いやすい位置に
            this.Top = SingletonManager.MainWindowSingleton.Top;
            this.Left = SingletonManager.MainWindowSingleton.Left;
        }

        async void ImageGetAsync(String URL)
        {
            BitmapFrame bf = null;
            Exception exp = null;
            await Task.Run(() =>
            {
                try
                {
                    WebClient wc = new WebClient();
                    Stream stream = wc.OpenRead(new Uri(URL));
                    Bitmap bitmap = new Bitmap(stream);
                    stream.Close();
                    Stream st = new MemoryStream();
                    bitmap.Save(st, ImageFormat.Bmp);
                    st.Seek(0, SeekOrigin.Begin);
                    bf = BitmapFrame.Create(st, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    st.Dispose();
                }
                catch (Exception ex)
                {
                    exp = ex;
                }
            });
            if (exp == null)
            {
                progBar.Visibility = Visibility.Collapsed;
                imageView.Source = bf;
            }
            else
            {
                await this.ShowMessageAsync("エラー", exp.Message);
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(URL);
        }

        private void MetroWindow_ContentRendered(object sender, EventArgs e)
        {
            //画像取得
            ImageGetAsync(URL);
        }
    }
}
