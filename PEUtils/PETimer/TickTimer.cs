using System;
using System.Collections.Concurrent;

namespace PEUtils {
    public class TickTimer : PETimer {

        private readonly DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private readonly ConcurrentDictionary<int, TickTask> taskDic = new ConcurrentDictionary<int, TickTask>();

        public override int AddTask(uint delay, Action<int> taskCB, Action<int> cancleCB, int count = 1) {
            int tid = GenerateTid();
            double startTime = GetUTCMilliseconds();
            double destTime = startTime + delay;
            TickTask task = new TickTask(tid, delay, startTime, destTime, taskCB, cancleCB, count);

            if (taskDic.TryAdd(tid, task)) {
                return tid;
            }
            else {
                return -1;
            }
        }

        public override bool DeleteTask(int delay) {
            return false;
        }

        public override void Rest() {
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
