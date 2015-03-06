using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Uzuki.Settings;

namespace Uzuki.BugReporter
{
    class ErrorReporter
    {
        public static void RegisterErrorReporter()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            StreamWriter stream = new StreamWriter(SettingManager.GetAppPath() + @"\" + "error.txt", true);
            stream.WriteLine("[ERROR]\r\n");
            stream.WriteLine("[message]\r\n" + ex.Message);
            stream.WriteLine("[source]\r\n" + ex.Source);
            stream.WriteLine("[stacktrace]\r\n" + ex.StackTrace);
            stream.WriteLine();
            stream.Close();
        }
    }
}
