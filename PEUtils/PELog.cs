using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

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
        private static StreamWriter streamWriter;
        public static void InitSetting(LogConfig cfg = null) {
            logCfg = cfg ?? new LogConfig();

            if (logCfg.loggerType == LoggerType.Console) {
                logger = new ConsoleLogger();
            }
            else {
                logger = new UnityLogger();
            }

            if (logCfg.enableSave == false) {
                return;
            }
            else if (logCfg.enableCover) {
                string path = logCfg.savePath + logCfg.saveName;
                try {
                    if (Directory.Exists(logCfg.savePath)) {
                        if (File.Exists(path)) {
                            File.Delete(path);
                        }
                    }
                    else {
                        Directory.CreateDirectory(logCfg.savePath);
                    }
                    streamWriter = File.AppendText(path);
                    streamWriter.AutoFlush = true;
                }
                catch {
                    streamWriter = null;
                }
            }
            else {
                string prefix = DateTime.Now.ToString("yyyyMMdd@HH-mm-ss");
                string path = logCfg.savePath + prefix + logCfg.saveName;
                try {
                    if (Directory.Exists(cfg.savePath) == false) {
                        Directory.CreateDirectory(logCfg.savePath);
                    }
                    streamWriter = File.AppendText(path);
                    streamWriter.AutoFlush = true;

                }
                catch {
                }
            }

        }
        public static void Log(string msg) {
            if (logCfg.enableLog == false) {
                return;
            }
            msg = DecorateLog($"{msg}", true);
            logger.Log(msg);
            if (logCfg.enableSave) {
                WriteToFile(msg);
            }
        }
        public static string DecorateLog(string msg, bool isTrac = false) {
            StringBuilder sb = new StringBuilder(logCfg.logPrefix, 100);
            if (logCfg.enableTime) {
                sb.Append(GetTime());
            }
            if (logCfg.enableThreadId) {
                sb.Append(GetThreadId());
            }

            sb.Append($" {logCfg.logSeparate} {msg}");

            if (isTrac) {
                sb.Append(GetStackTrace());
            }
            return sb.ToString();
        }
        private static string GetTime() {
            return $"  {DateTime.Now.ToString("hh:mm:ss--fff")}";
        }
        private static string GetThreadId() {
            return $"  ThreadID:{Thread.CurrentThread.ManagedThreadId}";
        }
        private static string GetStackTrace() {
            StackTrace st = new StackTrace(3, true);
            StringBuilder trackInfo = new StringBuilder(100);

            for (int i = 0; i < st.FrameCount; i++) {
                StackFrame sf = st.GetFrame(i);
                trackInfo.Append($"\n{sf.GetFileName()}::{sf.GetMethod()} line:{sf.GetFileLineNumber()}");
            }

            return $"\nStackTrace: {trackInfo}";
        }
        private static void WriteToFile(string msg) {
            if (streamWriter != null) {
                try {
                    streamWriter.WriteLine($"[L] { msg}");
                }
                catch {
                    streamWriter = null;
                }
            }
        }

    }
}