using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;

namespace Uzuki.UpdateChecker
{
    class UpdateNotify
    {
        const String UPDATE_CHECKER_JSON_URL = "https://raw.githubusercontent.com/kazukioishi/Uzuki/master/Uzuki/UpdateChecker/Updateinfo.json";
        public static void checkUpdate()
        {
            new Thread(updateCheckerThread).Start();
        }

        static void updateCheckerThread()
        {
            try
            {
                String rawJSON = new WebClient().DownloadString(UPDATE_CHECKER_JSON_URL);
                JObject jobject = (JObject)JsonConvert.DeserializeObject(rawJSON);
                Version newVersion = new Version(jobject["Version"].ToString());
                //Has update
                if (newVersion > Assembly.GetEntryAssembly().GetName().Version)
                {
                    Debug.WriteLine("Has update.");
                    Dialogs.MainWindow.SingletonManager.MainWindowSingleton.Dispatcher.Invoke(new Action(() =>
                    {
                        //アップデートのお知らせダイアログを表示する
                        showupdateDialog(jobject["UpdateLog"].ToString(), jobject["URL"].ToString());
                    }));
                }
                else
                {
                    Debug.WriteLine("No update found.");
                }
            }
            catch (Exception ex)
            {

            }
        }

        static async void showupdateDialog(String updateMesg, String URL)
        {
            MetroWindow metroWindow = Application.Current.MainWindow as MetroWindow;
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Inverted;
            MessageDialogResult result = await metroWindow.ShowMessageAsync("アップデートのお知らせ", updateMesg + "\n\nダウンロードページを開きますか?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                Process.Start(URL);
            }
        }
    }
}
