using System;
using System.Collections.Concurrent;

namespace PEUtils {
    internal class AsyncTimer : PETimer {


        private const string tidLock = "AsyncTimer_tidLock";
        private readonly ConcurrentDictionary<int, AsyncTask> taskDic;
        public override int AddTask(uint delay, Action<int> taskCB, Action<int> cancleCB, int count = 1) {
            throw new NotImplementedException();
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

        class AsyncTask {
            public int tid;
            public uint delay;
            public int count;
            public DateTime startTime;
            public ulong loopIndex;
            public Action<int> taskCb;
            public Action<int> cancleCb;

            public AsyncTask(int tid, uint delay, Action<int> taskCb, Action<int> cancleCb, int count) {
                this.tid = tid;
                this.delay = delay;
                startTime = DateTime.UtcNow;
                this.taskCb = taskCb;
                this.cancleCb = cancleCb;
                this.count = count;
            }
        }
    }
}
