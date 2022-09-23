using System;

namespace PEUtils {
    public enum LoggerType {
        Unity,
        Console
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

        public LoggerType LoggerType = LoggerType.Console;
        public string saveName = "ConsolePELog.txt";
        public string savePath = string.Format("{0}Logs\\", AppDomain.CurrentDomain.BaseDirectory);
    }
}
