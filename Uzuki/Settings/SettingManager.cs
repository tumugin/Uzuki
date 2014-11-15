using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        public String BBSMenuPath = "http://menu.2ch.net/bbsmenu.html";
        //履歴
        public ObservableCollection<_2ch.BBSThread> ThreadHistoryList = new ObservableCollection<_2ch.BBSThread>();

        public void SaveSettings()
        {
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
            return sm;
        }

        public static string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
