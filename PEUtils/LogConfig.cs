using System;

namespace PEUtils {
    public enum LoggerType {
        Unity,
        Console
    }
    public enum LogColor {
        None,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow
    }
    public class LogConfig {
        public string logPrefix = "#";
        public string logSeparate = ">>";

        public bool enableLog = true;
        public bool enableTime = true;
        public bool enableThreadId = true;
        public bool enableTrace = true;
        public bool enableSave = true;
        public bool enableCover = true;

        public LoggerType loggerType = LoggerType.Console;
        public string saveName = "ConsolePELog.txt";
        public string savePath = $"{AppDomain.CurrentDomain.BaseDirectory}Logs\\";
    }

    interface ILogger {
        void Log(string msg, LogColor color = LogColor.None);
        void Wain(string msg, LogColor color = LogColor.Yellow);
        void Error(string msg, LogColor color = LogColor.Red);
    }
}