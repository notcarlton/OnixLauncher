﻿using System;
using System.Diagnostics;
using System.IO;

namespace OnixLauncher
{
    public static class Log
    {
        private static string _logText = string.Empty;
        public static void CreateLog()
        {
            Directory.CreateDirectory(Utils.OnixPath + "\\Logs");
            if (File.Exists(Utils.OnixPath + "\\Logs\\Current.log"))
            {
                //save the log instead of deleting
                File.Move(Utils.OnixPath + "\\Logs\\Current.log", Utils.OnixPath + "\\Logs\\" + DateTime.Now.ToBinary() + ".log");
            }
            // File.Delete(Utils.OnixPath + "\\Logs\\Current.log"); kinda silly
        }

        public static void Write(string text)
        {
            Debug.WriteLine(text);
            _logText += text + Environment.NewLine;
            File.WriteAllText(Utils.OnixPath + "\\Logs\\Current.log", _logText);
        }
    }
}