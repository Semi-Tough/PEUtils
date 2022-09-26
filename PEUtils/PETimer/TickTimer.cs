using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace PEUtils {
    public class TickTimer : PETimer {

        private readonly DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private readonly ConcurrentDictionary<int, TickTask> taskDic = new ConcurrentDictionary<int, TickTask>();
        private readonly Thread timerThread;
        public TickTimer(int interval = 0) {
            if (interval > 0) {
                void StartTick() {
                    try {
                        while (true) {
                            UpdateTask();
                            Thread.Sleep(interval);
                        }
                    }
                    catch (ThreadAbortException e) {
                        wainFunc?.Invoke($"Tick Thread Abort: {e}.");
                    }
                }
                timerThread = new Thread(StartTick);
                timerThread.Start();

            }
        }

        public void UpdateTask() {
            double nowTime = GetUTCMilliseconds();
            foreach (KeyValuePair<int, TickTask> item in taskDic) {
                TickTask task = item.Value;
                ++task.loopIndex;

                if (nowTime < task.destTime) {
                    continue;
                }
                else {
                    if (task.count > 0) {
                        --task.count;
                        if (task.count == 0) {
                            FinishTask(task.tid);
                        }
                        else {
                            task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                            CallTaskCB(task.tid, task.taskCB);
                        }
                    }
                    else {
                        task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                        CallTaskCB(task.tid, task.taskCB);
                    }
                }
            }
        }

        private void FinishTask(int tid) {
            if (taskDic.TryRemove(tid, out TickTask task)) {
                CallTaskCB(task.tid, task.taskCB);
            }
        }
        private void CallTaskCB(int tid, Action<int> taskCB) {
            taskCB?.Invoke(tid);
        }

        public override int AddTask(uint delay, Action<int> taskCB, Action<int> cancleCB, int count = 1) {
            int tid = GenerateTid();
            double startTime = GetUTCMilliseconds();
            double destTime = startTime + delay;
            TickTask task = new TickTask(tid, delay, startTime, destTime, taskCB, cancleCB, count);

            if (taskDic.TryAdd(tid, task)) {
                return tid;
            }
            else {
                wainFunc?.Invoke($"{tid} already exist.");
                return -1;
            }
        }

        public override bool DeleteTask(int tid) {

            if (taskDic.TryRemove(tid, out TickTask task)) {
                task.cancleCB?.Invoke(tid);
                return true;
            }
            wainFunc?.Invoke($"{tid} remove failed.");
            return false;
        }

        public override void Rest() {
            taskDic.Clear();
            if (timerThread != null) {
                timerThread.Abort();
            }
        }

        protected override int GenerateTid() {
            return 0;
        }
        private double GetUTCMilliseconds() {
            TimeSpan ts = DateTime.UtcNow - startDateTime;
            return ts.TotalMilliseconds;
        }
        class TickTask {
            public int tid;
            public uint delay;
            public int count;
            public double destTime;
            public double startTime;
            public ulong loopIndex;
            public Action<int> taskCB;
            public Action<int> cancleCB;

            public TickTask(int tid, uint delay, double startTime, double destTime, Action<int> taskCB, Action<int> cancleCB, int count) {
                this.tid = tid;
                this.delay = delay;
                this.startTime = startTime;
                this.destTime = destTime;
                this.taskCB = taskCB;
                this.cancleCB = cancleCB;
                this.count = count;
            }
        }
    }
}
