using System;

namespace PEUtils {

    public class PELog {

        class ConsoleLogger : ILogger {
            public void Error(string msg, LogColor color = LogColor.Red) {
                WriteConsoleLog(msg, color);
            }

            public void Log(string msg, LogColor color = LogColor.None) {
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
            public void Error(string msg, LogColor color = LogColor.Red) {
            }

            public void Log(string msg, LogColor color = LogColor.None) {
            }

            public void Wain(string msg, LogColor color = LogColor.Yellow) {
            }
        }

    public static LogConfig cfg;
    private static ILogger logger;
    public static void InitSetting(LogConfig cfg = null) {
        PELog.cfg = cfg ?? new LogConfig();

        if (cfg.loggerType == LoggerType.Unity) {
            logger = new ConsoleLogger();
        }
        else {
            logger = new UnityLogger();
        }
    }
    }

}