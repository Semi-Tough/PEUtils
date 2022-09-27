﻿using System;

namespace PEUtils {
    public enum LoggerType
    {
        Unity,
        Console
    }
    public enum LogColor
    {
        None,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow
    }
    public class LogConfig
    {
        public string logPrefix = "#";
        public string logSeparate = ">>";

        public bool enableLog = true;
        public bool enableTime = true;
        public bool enableThreadId = true;
        public bool enableTrace = true;
        public bool enableSave = true;
        public bool enableCover = true;

        public LoggerType loggerType = LoggerType.Console;
        public string SaveName
        {
            get
            {
                if (loggerType == LoggerType.Console)
                {
                    return "ConsolePELog.txt";
                }
                else
                {
                    return "UnityPELog.txt";
                }
            }
            set
            {
                SaveName = value;
            }
        }
        public string SavePath
        {
            get
            {
                if (loggerType == LoggerType.Console)
                {
                    return $"{AppDomain.CurrentDomain.BaseDirectory}Logs\\";
                }
                else
                {
                    Type type = Type.GetType("UnityEngine.Application,UnityEngine");
                    return $"{type.GetProperty("persistentDataPath").GetValue(null)}/PELogs/";
                }
            }
            set
            {
                SaveName = value;
            }

        }
    }
    interface ILogger
    {
        void Log(string msg, LogColor color = LogColor.None);
        void Wain(string msg, LogColor color = LogColor.Yellow);
        void Error(string msg, LogColor color = LogColor.Red);
    }
}