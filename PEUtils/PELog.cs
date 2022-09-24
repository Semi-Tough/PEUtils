using System;

namespace PEUtils {

    public static class PELog {

        class ConsoleLogger : ILogger {
            public void Log(string msg, LogColor color = LogColor.None) {
                WriteConsoleLog(msg, color);
            }
            public void Error(string msg, LogColor color = LogColor.Red) {
                WriteConsoleLog(msg, color);
            }
            public void Wain(string msg, LogColor color = LogColor.Yellow) {
                WriteConsoleLog(msg, color);
            }
            private void WriteConsoleLog(string msg, LogColor color) {
                switch (color) {

                    case LogColor.Red:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogColor.Green:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogColor.Blue:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case LogColor.Magenta:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case LogColor.Yellow:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogColor.Cyan:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                }
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }
        class UnityLogger : ILogger {
            Type type = Type.GetType("UnityEngine.Debug,UnityEngine");
            public void Log(string msg, LogColor color = LogColor.None) {
                if (color != LogColor.None) {
                    msg = WriteUnityLog(msg, color);
                }
                type.GetMethod("Log", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }
            public void Error(string msg, LogColor color = LogColor.Red) {
                type.GetMethod("LogError", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
                WriteUnityLog(msg, color);
            }
            public void Wain(string msg, LogColor color = LogColor.Yellow) {
                type.GetMethod("LogWarning", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
                WriteUnityLog(msg, color);
            }
            private string WriteUnityLog(string msg, LogColor color) {
                switch (color) {
                    case LogColor.Red:
                        msg = $"<color=#FF0000>{msg}</color>";
                        break;
                    case LogColor.Green:
                        msg = $"<color=#00FF00>{msg}</color>";
                        break;
                    case LogColor.Blue:
                        msg = $"<color=#0000FF>{msg}</color>";
                        break;
                    case LogColor.Cyan:
                        msg = $"<color=#00FFFF>{msg}</color>";
                        break;
                    case LogColor.Magenta:
                        msg = $"<color=#FF00FF>{msg}</color>";
                        break;
                    case LogColor.Yellow:
                        msg = $"<color=#FFFF00>{msg}</color>";
                        break;
                }
                return msg;

            }

        }

        public static LogConfig logCfg;
        private static ILogger logger;
        public static void InitSetting(LogConfig cfg = null) {
            logCfg = cfg ?? new LogConfig();

            if (logCfg.loggerType == LoggerType.Console) {
                logger = new ConsoleLogger();
            }
            else {
                logger = new UnityLogger();
            }
        }

        public static void Log(string msg, params object[] args) {
            if (logCfg.enableLog == false) {
                return;
            }
            msg = String.Format(msg, args);
            logger.Log(msg);
        }
    }

}