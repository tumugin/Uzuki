using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Uzuki.Controls
{
    public class HyperlinkText : TextBlock
    {
        //イベントハンドラ
        public static event EventHandler OnLinkClick = delegate { };
        public static event EventHandler OnResClick = delegate { };

        private static Encoding m_Enc = Encoding.GetEncoding("Shift_JIS");

        public static readonly DependencyProperty ArticleContentProperty =
            DependencyProperty.RegisterAttached(
                "Inline",
                typeof(string),
                typeof(HyperlinkText),
                new PropertyMetadata(null, OnInlinePropertyChanged));

        public static string GetInline(TextBlock element)
        {
            return (element != null) ? element.GetValue(ArticleContentProperty) as string : string.Empty;
        }

        public static void SetInline(TextBlock element, string value)
        {
            if (element != null)
                element.SetValue(ArticleContentProperty, value);
        }

        private static void OnInlinePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var tb = obj as TextBlock;
            var msg = e.NewValue as string;
            if (tb == null || msg == null)
                return;

            // 末尾の改行コードを取り除く
            msg = msg.TrimEnd(new char[] { '\n', '\r' });

            // 改行位置の取得
            var nl = new List<int>();
            int i = 0;
            while ((i = msg.IndexOf("\r\n", i)) >= 0)
            {
                nl.Add(i - (nl.Count * 2));
                i += 2;
            }
            nl.Sort();

            // 正規表現でURLアドレスを検出
            var regex = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[A-Z0-9-.,_/?%~&=]*)?|>>(\d+)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var ReplyRegex = new Regex(@">>(\d+)");
            var text = msg.Replace("\r\n", "");
            var matchs = regex.Matches(text);
            if (matchs.Count > 0)
            {
                tb.Text = null;
                tb.Inlines.Clear();

                int pos = 0;
                int l = 0;
                foreach (Match match in matchs)
                {
                    var index = match.Groups[0].Index;
                    var length = match.Groups[0].Length;
                    var tag = match.Groups[0].Value;

                    // 文章前部の非リンク文字列を挿入
                    if (pos < index)
                    {
                        while (pos < text.Length)
                        {
                            if (nl.Count - l > 0 && nl[l] < index)
                            {
                                var buff = text.Substring(pos, nl[l] - pos);
                                tb.Inlines.Add(new Run(buff));
                                tb.Inlines.Add(new LineBreak());
                                pos = nl[l];
                                l++;
                            }
                            else
                            {
                                var buff = text.Substring(pos, index - pos);
                                tb.Inlines.Add(new Run(buff));
                                pos = index;
                                break;
                            }
                        }
                    }

                    // リンクの作成
                    var link = new Hyperlink();
                    link.TextDecorations = null;
                    //link.Foreground = tb.Foreground;
                    link.Foreground = (Brush)Application.Current.Resources["linkTextBrush"];
                    try
                    {
                        if (ReplyRegex.IsMatch(tag))
                        {
                            //解析できないリンク
                            link.Click += link_Click;
                        }
                        else
                        {
                            link.NavigateUri = new Uri(tag);
                        }
                    }
                    catch (Exception ex)
                    {
                        //解析できないリンク
                        link.Click += link_Click;
                    }
                    link.RequestNavigate += new RequestNavigateEventHandler(RequestNavigate);
                    link.MouseEnter += new MouseEventHandler(link_MouseEnter);
                    link.MouseLeave += new MouseEventHandler(link_MouseLeave);

                    while (pos < text.Length)
                    {
                        if (nl.Count - l > 0 && nl[l] < index + length)
                        {
                            var buff = text.Substring(pos, nl[l] - pos);
                            link.Inlines.Add(new Run(buff));
                            link.Inlines.Add(new LineBreak());
                            pos = nl[l];
                            l++;
                        }
                        else
                        {
                            var buff = text.Substring(pos, index + length - pos);
                            link.Inlines.Add(new Run(buff));
                            pos = index + length;
                            break;
                        }
                    }

                    // Hyperlinkの追加
                    tb.Inlines.Add(link);
                }
                // 文章後部の非リンク文字列を挿入
                while (pos < text.Length)
                {
                    if (nl.Count - l > 0)
                    {
                        var buff = text.Substring(pos, nl[l] - pos);
                        tb.Inlines.Add(new Run(buff));
                        tb.Inlines.Add(new LineBreak());
                        pos = nl[l];
                        l++;
                    }
                    else
                    {
                        var buff = text.Substring(pos, text.Length - pos);
                        tb.Inlines.Add(new Run(buff));
                        pos = text.Length;
                        break;
                    }
                }
            }
            else
                tb.Text = msg;
        }

        static void link_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)sender;
            if(link.NavigateUri == null) OnResClick(sender, e);
        }

        private static void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                // URLリンクが選択された場合、既定のアプリケーション(ブラウザ)を起動する
                //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                OnLinkClick(sender, e);
                // 処理した場合はtrueを返す
                e.Handled = true;
            }
            catch
            {
                // 特に何もしない
            }
        }

        private static void link_MouseEnter(object sender, MouseEventArgs e)
        {
            var link = sender as Hyperlink;
            if (link == null)
                return;

            // リンクにカーソルを当てたときは文字色を赤くする
            //link.Foreground = Brushes.Red;
        }

        private static void link_MouseLeave(object sender, MouseEventArgs e)
        {
            var link = sender as Hyperlink;
            var parent = link.Parent as TextBlock;
            if (link == null || parent == null)
                return;

            // リンクからカーソルが離れたときは文字色をデフォルトカラーに戻す
            //link.Foreground = parent.Foreground;
        }
    }
}
