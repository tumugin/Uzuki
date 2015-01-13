using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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

namespace Uzuki.Dialogs.SubDialogs
{
    /// <summary>
    /// ImagePreviewDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ImagePreviewDialog : MetroWindow
    {
        public ImagePreviewDialog()
        {
            InitializeComponent();
        }
        public void ImageGet(String url)
        {
            ImagegetThread it = new ImagegetThread();
            it.URL = url;
            it.image = imageView;
            it.Window = this;
            Thread th = new Thread(it.getImage);
            th.Start();
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
                    Stream stream = wc.OpenRead(URL);
                    Bitmap bitmap = new Bitmap(stream);
                    stream.Close();
                    Stream st = new MemoryStream();
                    bitmap.Save(st, ImageFormat.Bmp);
                    st.Seek(0, SeekOrigin.Begin);
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        image.Source = BitmapFrame.Create(st, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }));
                    st.Dispose();
                }
                catch { }

            }
        }
    }
}
