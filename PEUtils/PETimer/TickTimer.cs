﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace PEUtils {
    public class TickTimer : PETimer {

        private readonly Thread timerThread;
        private const string tidLock = "PETimer_tidLock";
        private readonly bool setHandle;
        private readonly ConcurrentQueue<TickTaskPack> packQue;
        private readonly ConcurrentDictionary<int, TickTask> taskDic;
        private readonly DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public TickTimer(int interval = 0, bool setHandle = true) {
            taskDic = new ConcurrentDictionary<int, TickTask>();
            this.setHandle = setHandle;
            if (setHandle) {
                packQue = new ConcurrentQueue<TickTaskPack>();
            }

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
        public override int AddTask(uint delay, Action<int> taskCb, Action<int> cancleCb, int count = 1) {
            int tid = GenerateTid();
            double startTime = GetUtcMilliseconds();
            double destTime = startTime + delay;
            TickTask task = new TickTask(tid, delay, startTime, destTime, taskCb, cancleCb, count);

            if (taskDic.TryAdd(tid, task)) {
                return tid;
            }

            wainFunc?.Invoke($"{tid} already exist.");
            return -1;
        }
        public override bool DeleteTask(int tid) {

            if (taskDic.TryRemove(tid, out TickTask task)) {
                if (setHandle&&task.cancleCb!=null) {
                    packQue.Enqueue(new TickTaskPack(tid, task.cancleCb));
                }
                else {
                    task.cancleCb?.Invoke(tid);
                    return true;
                }
            }
            wainFunc?.Invoke($"{tid} remove failed.");
            return false;
        }

        public void HandleTask() {
            while (packQue!=null&&packQue.Count>0) {
                if(packQue.TryDequeue(out TickTaskPack pack)) {
                    pack.cb?.Invoke(pack.tid);
                }
                else {
                    errorFunc?.Invoke($"packQue Dequeue Data Error");
                }

            }
        }
        public void UpdateTask() {
            double nowTime = GetUtcMilliseconds();
            foreach (KeyValuePair<int, TickTask> item in taskDic) {
                TickTask task = item.Value;
                ++task.loopIndex;

                if (nowTime < task.destTime) {
                    continue;
                }

                if (task.count > 0) {
                    --task.count;
                    if (task.count == 0) {
                        FinishTask(task.tid);
                    }
                    else {
                        task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                        CallTaskCb(task.tid, task.taskCb);
                    }
                }
                else {
                    task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                    CallTaskCb(task.tid, task.taskCb);
                }
            }
        }
        private void FinishTask(int tid) {
            if (taskDic.TryRemove(tid, out TickTask task)) {
                CallTaskCb(task.tid, task.taskCb);
            }
        }
        private void CallTaskCb(int tid, Action<int> taskCb) {
            if (setHandle) {
                packQue.Enqueue(new TickTaskPack(tid, taskCb));
            }
            else {
                taskCb?.Invoke(tid);
            }
        }
        public override void Rest() {
            if (!packQue.IsEmpty) {
                wainFunc?.Invoke($"CallBack is not Empty.");
            }
            taskDic.Clear();
            timerThread?.Abort();
        }
        protected override int GenerateTid() {
            lock (tidLock) {
                while (true) {
                    ++globalTid;
                    if (globalTid == int.MaxValue) {
                        globalTid = 0;
                    }
                    if (!taskDic.ContainsKey(globalTid)) {
                        return globalTid;
                    }
                }
            }
        }
        private double GetUtcMilliseconds() {
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
            public Action<int> taskCb;
            public Action<int> cancleCb;

            public TickTask(int tid, uint delay, double startTime, double destTime, Action<int> taskCb, Action<int> cancleCb, int count) {
                this.tid = tid;
                this.delay = delay;
                this.startTime = startTime;
                this.destTime = destTime;
                this.taskCb = taskCb;
                this.cancleCb = cancleCb;
                this.count = count;
            }
        }

        private class TickTaskPack {
            public int tid;
            public Action<int> cb;
            public TickTaskPack(int tid, Action<int> cb) {
                this.tid = tid;
                this.cb = cb;
            }
        }
    }
}
