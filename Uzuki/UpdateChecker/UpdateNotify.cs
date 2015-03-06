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

namespace Uzuki.UpdateChecker
{
    static class UpdateNotify
    {
        const String UPDATE_CHECKER_JSON_URL = "";
        void checkUpdate()
        {
            new Thread(updateCheckerThread).Start();
        }

        void updateCheckerThread()
        {
            try
            {
                String rawJSON = new WebClient().DownloadString(UPDATE_CHECKER_JSON_URL);
                JObject jobject = (JObject)JsonConvert.DeserializeObject(rawJSON);
                Version newVersion = new Version(jobject["Version"].ToString());
                //Has update
                if (newVersion > Assembly.GetEntryAssembly().GetName().Version)
                {
                    Dialogs.MainWindow.SingletonManager.MainWindowSingleton.Dispatcher.Invoke(new Action(() =>
                    {
                        //アップデートのお知らせダイアログを表示する
                    }));
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
