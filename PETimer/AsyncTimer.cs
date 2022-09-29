using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PEUtils {
    internal class AsyncTimer : PETimer {


        private const string tidLock = "AsyncTimer_tidLock";
        private readonly ConcurrentDictionary<int, AsyncTask> taskDic;
        public override int AddTask(uint delay, Action<int> taskCb, Action<int> cancleCb, int count = 1) {
            int tid = GenerateTid();
            AsyncTask task = new AsyncTask(tid, delay, taskCb, cancleCb, count);
            RunTaskInPool(task);
            if (taskDic.TryAdd(tid, task)) {
                return tid;
            }
            else {
                wainFunc?.Invoke($"key:{tid} already exist");
                return -1;
            }
        }

        public override bool DeleteTask(int tid) {
            throw new NotImplementedException();
        }

        public override void Rest() {
            throw new NotImplementedException();
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
        private void RunTaskInPool(AsyncTask task) {
            Task.Run(async () => {
                if (task.count > 0) {
                    do {
                        --task.count;
                        ++task.loopIndex;
                        int delay = (int)(task.delay + task.fixDelta);
                        if (delay > 0) {
                            await Task.Delay(delay, task.ct);
                        }
                        else {
                            errorFunc?.Invoke($"tid:{task.tid} delayTime error.");
                        }
                        TimeSpan ts = DateTime.UtcNow - task.startTime;
                        task.fixDelta = (int)(task.delay * task.loopIndex - ts.TotalMilliseconds);
                        CallBackTaskCb(task);
                    } while (task.count > 0);
                }
                else {
                    while (true) {
                        --task.count;
                        ++task.loopIndex;
                        int delay = (int)(task.delay + task.fixDelta);
                        if (delay > 0) {
                            await Task.Delay(delay, task.ct);
                        }
                        else {
                            errorFunc?.Invoke($"tid:{task.tid} delayTime error.");
                        }
                        TimeSpan ts = DateTime.UtcNow - task.startTime;
                        task.fixDelta = (int)(task.delay * task.loopIndex - ts.TotalMilliseconds);
                        CallBackTaskCb(task);
                    }
                }


            });
        }
        private void CallBackTaskCb(AsyncTask task) {

        }
        class AsyncTask {
            public int tid;
            public uint delay;
            public int count;
            public DateTime startTime;
            public ulong loopIndex;
            public int fixDelta;
            public Action<int> taskCb;
            public Action<int> cancleCb;
            public CancellationTokenSource cts;
            public CancellationToken ct;
            public AsyncTask(int tid, uint delay, Action<int> taskCb, Action<int> cancleCb, int count) {
                this.tid = tid;
                this.delay = delay;
                startTime = DateTime.UtcNow;
                this.taskCb = taskCb;
                this.cancleCb = cancleCb;
                this.count = count;
                loopIndex = 0;
                fixDelta = 0;
                cts = new CancellationTokenSource();
                ct = cts.Token;
            }
        }
    }
}
