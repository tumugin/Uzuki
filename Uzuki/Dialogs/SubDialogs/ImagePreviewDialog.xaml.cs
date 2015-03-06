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

namespace Uzuki.Dialogs.SubDialogs
{
    /// <summary>
    /// ImagePreviewDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ImagePreviewDialog : MetroWindow
    {
        String browserurl;

        public ImagePreviewDialog()
        {
            InitializeComponent();
            this.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Inverted;
        }
        public void ImageGet(String url)
        {
            ImagegetThread it = new ImagegetThread();
            it.URL = url;
            it.image = imageView;
            it.Window = this;
            Thread th = new Thread(it.getImage);
            th.Start();
            browserurl = it.URL;
        }

        class ImagegetThread
        {
            public String URL;
            public System.Windows.Controls.Image image;
            public ImagePreviewDialog Window;
            public void getImage()
            {
                try
                {
                    WebClient wc = new WebClient();
                    wc.OpenReadCompleted += wc_OpenReadCompleted;
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.OpenReadAsync(new Uri(URL));
                }
                catch(Exception ex) {
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        ((MetroWindow)Window).ShowMessageAsync("エラー", ex.Message);
                        //Window.Close();
                    }));
                }
            }

            void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                //イベントが飛んでこない訴訟
                Window.Dispatcher.Invoke(new Action(() =>
                {
                    Window.progBar.Value = e.ProgressPercentage;
                    Debug.WriteLine(e.ProgressPercentage);
                }));
            }

            void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        ((MetroWindow)Window).ShowMessageAsync("エラー",e.Error.Message);
                        //Window.Close();
                    }));
                    return;
                }
                try
                {
                    Stream stream = e.Result;
                    Bitmap bitmap = new Bitmap(stream);
                    stream.Close();
                    Stream st = new MemoryStream();
                    bitmap.Save(st, ImageFormat.Bmp);
                    st.Seek(0, SeekOrigin.Begin);
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        Window.progBar.Visibility = Visibility.Collapsed;
                        image.Source = BitmapFrame.Create(st, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }));
                    st.Dispose();
                }
                catch(Exception ex) {
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        ((MetroWindow)Window).ShowMessageAsync("エラー",ex.Message);
                        //Window.Close();
                    }));
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(browserurl);
        }
    }
}
