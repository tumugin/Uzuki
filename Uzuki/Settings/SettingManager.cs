using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Xml.Serialization;
using Uzuki.Dialogs.MainWindow;

namespace Uzuki.Settings
{
    public class SettingManager
    {
        [System.Xml.Serialization.XmlIgnore]
        public const String SETTING_FILE_NAME = "setting.xml";
        [System.Xml.Serialization.XmlIgnore]
        public const String COOKIE_FILE_NAME = "cookie";
        //CookieContainerはXMLシリアライズ出来ない(許さない)
        [System.Xml.Serialization.XmlIgnore]
        public CookieContainer Cookie = new CookieContainer();
        public String BBSMenuPath { get; set; } = "http://2ch.sc/bbsmenu.html";
        public bool UseBlackTheme { get; set; } = true;
        //マズイですよしぇんぱい！
        public String NetAPIKEY { get; set; } = "";
        public String NetHMKEY { get; set; } = "";
        public String NetX2chUA { get; set; } = "";
        public String Net2chUserAgent { get; set; } = "";
        //ウィンドウ位置
        public Ayana.WINDOWPLACEMENT WindowPlacement;
        public bool hasWindowSetValue = false;
        //履歴
        public ObservableCollection<_2ch.BBSThread> ThreadHistoryList = new ObservableCollection<_2ch.BBSThread>();
        //ツリー表示orレス順表示
        public bool EnableTreeView { get; set; } = true;
        public bool EnableResView { get; set; } = false;
        //NG list
        public ObservableCollection<String> NGIDCollection = new ObservableCollection<String>();

        public SettingManager(){
        }
        public void SaveSettings()
        {
            //彩奈
            var hwnd = new WindowInteropHelper(SingletonManager.MainWindowSingleton).Handle;
            Ayana.NativeMethods.GetWindowPlacement(hwnd, out WindowPlacement);
            hasWindowSetValue = true;
            //シリアライズ
            XmlSerializer serializer = new XmlSerializer(typeof(SettingManager));
            StreamWriter sw = new StreamWriter(GetAppPath() + @"\" + SETTING_FILE_NAME, false, new System.Text.UTF8Encoding(false));
            serializer.Serialize(sw, this);
            sw.Close();
            SaveCookie();
        }

        public void SaveCookie()
        {
            Stream stream = File.Create(GetAppPath() + @"\" + COOKIE_FILE_NAME);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, Cookie);
            stream.Close();
        }

        public void LoadCookie()
        {
            if (File.Exists(GetAppPath() + @"\" + COOKIE_FILE_NAME))
            {
                Stream stream = File.Open(GetAppPath() + @"\" + COOKIE_FILE_NAME, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                Cookie = (CookieContainer)formatter.Deserialize(stream);
                stream.Close();
            }
        }

        public static SettingManager LoadSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingManager));
            if (!System.IO.File.Exists(GetAppPath() + @"\" + SETTING_FILE_NAME))
            {
                //ファイルがねぇよ
                return new SettingManager();
            }
            StreamReader sw = new StreamReader(GetAppPath() + @"\" + SETTING_FILE_NAME, new System.Text.UTF8Encoding(false));
            SettingManager sm = (SettingManager)serializer.Deserialize(sw);
            sw.Close();
            sm.LoadCookie();
            //ウィンドウ位置ロード
            if (sm.hasWindowSetValue) Ayana.NativeMethods.SetWindowPlacement(new WindowInteropHelper(SingletonManager.MainWindowSingleton).Handle, ref sm.WindowPlacement);
            return sm;
        }

        public static string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
