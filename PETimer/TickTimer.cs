using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PEUtils {
    public class TickTimer : PETimer {

        private readonly bool setHandle;
        private readonly Thread timerThread;
        private const string tidLock = "TickTimer_tidLock";
        private readonly ConcurrentQueue<TickTaskPack> packQue;
        private readonly ConcurrentDictionary<int, TickTask> taskDic;
        private readonly DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public TickTimer(int interval = 0, bool setHandle = true) {
            this.setHandle = setHandle;
            taskDic = new ConcurrentDictionary<int, TickTask>();
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
                        errorFunc?.Invoke($"Tick Thread Abort: {e}.");
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
            else {
                wainFunc?.Invoke($"key:{tid} already exist.");
                return -1;
            }
        }
        public override bool DeleteTask(int tid) {

            if (taskDic.TryRemove(tid, out TickTask task)) {
                if (setHandle && task.cancleCb != null) {
                    packQue.Enqueue(new TickTaskPack(tid, task.cancleCb));
                }
                else {
                    task.cancleCb?.Invoke(tid);
                }
                logFunc?.Invoke($"Remove tid:{tid} in taskDic success.");
                return true;
            }
            else {
                wainFunc?.Invoke($"Remove task: {tid} in taskDic failed.");
                return false;
            }
        }
        public void HandleTask() {
            while (packQue != null && !packQue.IsEmpty) {
                if (packQue.TryDequeue(out TickTaskPack pack)) {
                    pack.cb?.Invoke(pack.tid);
                }
                else {
                    wainFunc?.Invoke($"Dequeue task:{pack.tid} in packQue failed.");
                }

            }
        }
        public void UpdateTask() {
            double nowTime = GetUtcMilliseconds();
            foreach (var item in taskDic) {
                TickTask task = item.Value;

                if (nowTime < task.destTime) {
                    continue;
                }

                ++task.loopIndex;
                if (task.count > 0) {
                    --task.count;
                    task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                    CallTaskCb(task.tid, task.taskCb);
                    if (task.count == 0) {
                        FinishTask(task.tid);
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
                logFunc?.Invoke($"Task tid:{tid} is completion.");
            }
            else {
                wainFunc?.Invoke($"Remove task: {tid} in taskDic failed.");
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
            if (packQue != null && !packQue.IsEmpty) {
                wainFunc?.Invoke($"CallBack is not Empty.");
            }
            taskDic.Clear();
            globalTid = 0;
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
