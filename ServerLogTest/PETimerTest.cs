using PEUtils;

namespace PEUtilsTest {
    internal class PETimerTest {
        public void TickTimerTest() {
            PELog.InitSetting();
            TickTimer tickTimer = new TickTimer(10, false) {
                logFunc = PELog.Log,
                wainFunc = PELog.Wain,
                errorFunc = PELog.Error
            };
            uint interval = 66;
            int count = 50;
            int sum = 0;
            int tid = 0;
            Task.Run(async () => {
                await Task.Delay(2000);
                DateTime startTime = DateTime.UtcNow;
                tid = tickTimer.AddTask(
                    interval,
                    (int tid) => {
                        DateTime nowTime = DateTime.UtcNow;
                        TimeSpan ts = nowTime - startTime;
                        startTime = nowTime;
                        int delta = (int)(ts.TotalMilliseconds - interval);
                        PELog.ColorLog($"间隔差: {delta}", LogColor.Blue);
                        //sum += Math.Abs(delta);
                        sum+=delta;
                        PELog.ColorLog($"tid: {tid} work", LogColor.Green);

                    },
                    (int tid) => {
                        PELog.ColorLog($"tid: {tid} cancle", LogColor.Yellow);
                    },
                    count
                    );
            });

            while (true) {
                string? str = Console.ReadLine();
                if (str == "cal") {
                    PELog.ColorLog($"计算平均偏差: {sum * 1.0f / count}", LogColor.Red);
                }
                else if (str == "del") {
                    PELog.ColorLog($"取消任务: {tid}", LogColor.Red);
                    tickTimer.DeleteTask(tid);
                }
            }
        }
    }
}
