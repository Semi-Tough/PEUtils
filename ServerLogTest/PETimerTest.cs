namespace PEUtils {
    internal class PETimerTest {

        #region TickTimer
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
                        sum += delta;
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

        public void TickTimerTestHandle() {
            PELog.InitSetting();
            TickTimer tickTimer = new TickTimer(10, true) {
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
                        sum += delta;
                        PELog.ColorLog($"tid: {tid} work", LogColor.Green);

                    },
                    (int tid) => {
                        PELog.ColorLog($"tid: {tid} cancle", LogColor.Yellow);
                    },
                    count
                    );
            });

            Task.Run(async () => {
                while (true) {
                    tickTimer.HandleTask();
                    await Task.Delay(2);
                }
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

        public void TickTimerTestUpdate() {
            PELog.InitSetting();
            TickTimer tickTimer = new TickTimer(0, false) {
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
                        sum += delta;
                        PELog.ColorLog($"tid: {tid} work", LogColor.Green);

                    },
                    (int tid) => {
                        PELog.ColorLog($"tid: {tid} cancle", LogColor.Yellow);
                    },
                    count
                    );
            });

            Task.Run(async () => {
                while (true) {
                    tickTimer.UpdateTask();
                    await Task.Delay(2);
                }
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

        public void TickTimerTestUpdateHandle() {
            PELog.InitSetting();
            TickTimer tickTimer = new TickTimer(0, true) {
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
                        sum += delta;
                        PELog.ColorLog($"tid: {tid} work", LogColor.Green);

                    },
                    (int tid) => {
                        PELog.ColorLog($"tid: {tid} cancle", LogColor.Yellow);
                    },
                    count
                    );
            });

            Task.Run(async () => {
                while (true) {
                    tickTimer.HandleTask();
                    tickTimer.UpdateTask();
                    await Task.Delay(2);
                }
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

        #endregion

        #region AsyncTimer
        public void AsyncTimerTest() {
            PELog.InitSetting();
            AsyncTimer timer = new AsyncTimer(false) {
                logFunc = PELog.Log,
                wainFunc = PELog.Wain,
                errorFunc = PELog.Error
            };
            uint interval = 66;
            int count = 10;
            int sum = 0;
            int tid = 0;
            Task.Run(async () => {
                await Task.Delay(2000);
                DateTime startTime = DateTime.UtcNow;
                tid = timer.AddTask(
                    interval,
                    (int tid) => {
                        DateTime nowTime = DateTime.UtcNow;
                        TimeSpan ts = nowTime - startTime;
                        startTime = nowTime;
                        int delta = (int)(ts.TotalMilliseconds - interval);
                        PELog.ColorLog($"间隔差: {delta}", LogColor.Blue);
                        sum += delta;
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
                    timer.DeleteTask(tid);
                }
            }
        }

        public void AsyncTimerTestHandle() {
            PELog.InitSetting();
            AsyncTimer timer = new AsyncTimer(true) {
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
                tid = timer.AddTask(
                    interval,
                    (int tid) => {
                        DateTime nowTime = DateTime.UtcNow;
                        TimeSpan ts = nowTime - startTime;
                        startTime = nowTime;
                        int delta = (int)(ts.TotalMilliseconds - interval);
                        PELog.ColorLog($"间隔差: {delta}", LogColor.Blue);
                        sum += delta;
                        PELog.ColorLog($"tid: {tid} work", LogColor.Green);

                    },
                    (int tid) => {
                        PELog.ColorLog($"tid: {tid} cancle", LogColor.Yellow);
                    },
                    count
                    );
            });

            Task.Run(async () => {
                while (true) {
                    timer.HandleTask();
                    await Task.Delay(2);
                }
            });

            while (true) {
                string? str = Console.ReadLine();
                if (str == "cal") {
                    PELog.ColorLog($"计算平均偏差: {sum * 1.0f / count}", LogColor.Red);
                }
                else if (str == "del") {
                    PELog.ColorLog($"取消任务: {tid}", LogColor.Red);
                    timer.DeleteTask(tid);
                }
            }




        }

        #endregion

        #region FrameTimer
        public void FrameTimerTest() {
            PELog.InitSetting();
            FrameTimer timer = new FrameTimer(100) {
                logFunc = PELog.Log,
                wainFunc = PELog.Wain,
                errorFunc = PELog.Error
            };
            uint interval = 10;
            int count = 10;
            int sum = 0;
            int tid = 0;
            Task.Run(async () => {
                await Task.Delay(2000);
                DateTime startTime = DateTime.UtcNow;
                tid = timer.AddTask(
                    interval,
                    (int tid) => {
                        PELog.ColorLog($"tid: {tid} work", LogColor.Green);
                    },
                    (int tid) => {
                        PELog.ColorLog($"tid: {tid} cancle", LogColor.Yellow);
                    },
                    count
                    );
                while (true) {
                    timer.UpdateTask();
                    Thread.Sleep(66);
                }
            });


            while (true) {
                string? str = Console.ReadLine();
                if (str == "cal") {
                    PELog.ColorLog($"计算平均偏差: {sum * 1.0f / count}", LogColor.Red);
                }
                else if (str == "del") {
                    PELog.ColorLog($"取消任务: {tid}", LogColor.Red);
                    timer.DeleteTask(tid);
                }
            }
        }

        #endregion
    }
}
