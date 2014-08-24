using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Uzuki.Settings
{
    public class SettingManager
    {
        [System.Xml.Serialization.XmlIgnore]
        public const String SETTING_FILE_NAME = "setting.xml";
        public CookieContainer Cookie = new CookieContainer();
        public List<_2ch.Objects.ThreadHistoryObj> ThreadHistory = new List<_2ch.Objects.ThreadHistoryObj>();
        public String BBSMenuPath = "http://2ch.sc/bbsmenu.html";

        public void SaveSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingManager));
            StreamWriter sw = new StreamWriter(GetAppPath() + @"\" + SETTING_FILE_NAME, false, new System.Text.UTF8Encoding(false));
            serializer.Serialize(sw, this);
            sw.Close();
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
            return sm;
        }

        public static string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
