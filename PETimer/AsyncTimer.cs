using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PEUtils {
    public class AsyncTimer : PETimer {

        private readonly bool setHandle;
        private const string tidLock = "AsyncTimer_tidLock";
        private readonly ConcurrentQueue<AsyncTaskPack> packQue;
        private readonly ConcurrentDictionary<int, AsyncTask> taskDic;

        public AsyncTimer(bool setHandle) {
            this.setHandle = setHandle;
            taskDic = new ConcurrentDictionary<int, AsyncTask>();
            if (setHandle) {
                packQue = new ConcurrentQueue<AsyncTaskPack>();
            }
        }

        public override int AddTask(uint delay, Action<int> taskCb, Action<int> cancleCb, int count = 1) {
            int tid = GenerateTid();
            AsyncTask task = new AsyncTask(tid, delay, taskCb, cancleCb, count);
            if (taskDic.TryAdd(tid, task)) {
                RunTaskInPool(task);
                return tid;
            }
            else {
                wainFunc?.Invoke($"key:{tid} already exist");
                return -1;
            }
        }

        public override bool DeleteTask(int tid) {

            if (taskDic.TryRemove(tid, out AsyncTask task)) {
                if (setHandle && task.cancleCb != null) {
                    packQue.Enqueue(new AsyncTaskPack(tid, task.cancleCb));
                }
                else {
                    task.cancleCb?.Invoke(tid);
                }
                task.cts.Cancel();
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
                if (packQue.TryDequeue(out AsyncTaskPack pack)) {
                    pack.cb?.Invoke(pack.tid);
                }
                else {
                    wainFunc?.Invoke($"Dequeue task:{pack.tid} in packQue failed.");
                }
            }
        }
        private void RunTaskInPool(AsyncTask task) {
            Task.Run(async () => {
                if (task.count > 0) {
                    while (task.count > 0) {
                        --task.count;
                        ++task.loopIndex;
                        int delay = (int)(task.delay + task.fixDelta);
                        if (delay > 0) {
                            await Task.Delay(delay, task.ct);
                        }
                        else {
                            wainFunc?.Invoke($"tid:{task.tid} delayTime error.");
                        }

                        if (task.count == 0) {
                            FinishTask(task.tid);
                        }
                        else {
                            TimeSpan ts = DateTime.UtcNow - task.startTime;
                            task.fixDelta = (int)(task.delay * task.loopIndex - ts.TotalMilliseconds);
                            CallBackTaskCb(task);
                        }
                    }
                }
                else {
                    while (true) {
                        ++task.loopIndex;
                        int delay = (int)(task.delay + task.fixDelta);
                        if (delay > 0) {
                            await Task.Delay(delay, task.ct);
                        }
                        else {
                            wainFunc?.Invoke($"tid:{task.tid} delayTime error.");
                        }
                        TimeSpan ts = DateTime.UtcNow - task.startTime;
                        task.fixDelta = (int)(task.delay * task.loopIndex - ts.TotalMilliseconds);
                        CallBackTaskCb(task);
                    }
                }
            }, task.ct);
        }
        private void FinishTask(int tid) {
            if (taskDic.TryRemove(tid, out AsyncTask task)) {
                CallBackTaskCb(task);
            }
            else {
                wainFunc?.Invoke($"Remove task: {tid} in taskDic failed.");
            }
        }
        private void CallBackTaskCb(AsyncTask task) {
            if (setHandle) {
                packQue.Enqueue(new AsyncTaskPack(task.tid, task.taskCb));
            }
            else {
                task.taskCb?.Invoke(task.tid);
            }
        }
        public override void Rest() {
            if (packQue != null && !packQue.IsEmpty) {
                wainFunc?.Invoke($"CallBack is not Empty.");
            }
            taskDic.Clear();
            globalTid = 0;

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
        class AsyncTaskPack {
            public int tid;
            public Action<int> cb;
            public AsyncTaskPack(int tid, Action<int> cb) {
                this.tid = tid;
                this.cb = cb;
            }
        }
    }
}
