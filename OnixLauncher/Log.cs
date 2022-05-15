﻿using System;
using System.IO;

namespace OnixLauncher
{
    public static class Log
    {
        public static string LogPath = Utils.OnixPath + "\\Logs";

        private static string _logText = string.Empty;

        public static void CreateLog()
        {
            Directory.CreateDirectory(LogPath + "\\Previous");
            if (File.Exists(LogPath + "\\Current.log"))
            {
                File.Move(LogPath + "\\Current.log", LogPath + "\\Previous\\Old" + DateTime.Now.ToBinary() + ".log");
            }
        }

        public static void Write(string text)
        {
            Console.WriteLine(text);
            _logText += text + Environment.NewLine;
            File.WriteAllText(LogPath + "\\Current.log", _logText);
        }
    }
}