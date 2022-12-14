using System;
using System.Collections.Generic;

namespace PEUtils {
    public class FrameTimer : PETimer {

        private ulong nowFrame;
        private const string tidLock = "TickTimer_tidLock";
        private List<int> tidList;
        private readonly Dictionary<int, FrameTask> taskDic;

        public FrameTimer(ulong frameId) {
            nowFrame = frameId;
            tidList = new List<int>();
            taskDic = new Dictionary<int, FrameTask>();
        }
        public override int AddTask(uint delay, Action<int> taskCb, Action<int> cancleCb, int count = 1) {
            int tid = GenerateTid();
            ulong destFrame = nowFrame + delay;
            FrameTask task = new FrameTask(tid, delay, destFrame, taskCb, cancleCb, count);
            if (taskDic.ContainsKey(tid)) {
                wainFunc?.Invoke($"key:{tid} already exist.");
                return -1;
            }
            else {
                taskDic.Add(tid, task);
                return tid;
            }
        }
        public override bool DeleteTask(int tid) {
            if (taskDic.TryGetValue(tid, out FrameTask task)) {
                if (taskDic.Remove(tid)) {
                    task.cancleCb?.Invoke(tid);
                    logFunc?.Invoke($"Remove tid:{tid} in taskDic success.");
                    return true;
                }
                else {
                    wainFunc?.Invoke($"Remove task: {tid} in taskDic failed.");
                    return false;
                }
            }
            else {
                wainFunc?.Invoke($" task: {tid} is not exist.");
                return false;
            }
        }
        public override void Rest() {
            taskDic.Clear();
            tidList.Clear();
            globalTid = 0;
        }
        public void UpdateTask() {
            ++nowFrame;
            tidList.Clear();
            foreach (var item in taskDic) {
                FrameTask task = item.Value;
                if (task.destFrame <= nowFrame) {
                    task.taskCb?.Invoke(task.tid);
                    task.destFrame += task.delay;
                    --task.count;
                    if (task.count == 0) {
                        tidList.Add(task.tid);
                    }
                }
            }
            FinishTask();

        }
        private void FinishTask() {
            if (tidList.Count <= 0) return;
            for (int i = 0; i < tidList.Count; i++) {
                if (taskDic.Remove(tidList[i])) {
                    logFunc?.Invoke($"Task tid:{tidList[i]} is completion.");
                }
                else {
                    wainFunc?.Invoke($"Remove task: {tidList[i]} in taskDic failed.");
                }
            }
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
        class FrameTask {
            public int tid;
            public uint delay;
            public int count;
            public ulong destFrame;
            public Action<int> taskCb;
            public Action<int> cancleCb;

            public FrameTask(int tid, uint delay, ulong destFrame, Action<int> taskCb, Action<int> cancleCb, int count) {
                this.tid = tid;
                this.delay = delay;
                this.destFrame = destFrame;
                this.taskCb = taskCb;
                this.cancleCb = cancleCb;
                this.count = count;
            }
        }

    }
}
