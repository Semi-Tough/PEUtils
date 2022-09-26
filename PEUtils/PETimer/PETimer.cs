using System;

namespace PEUtils {
    public abstract class PETimer {

        public Action<string> logFunc;
        public Action<string> wainFunc;
        public Action<string> errorFunc;

        protected int tid = 0;
        public abstract int AddTask(uint delay, Action<int> taskCB, Action<int> cancleCB, int count = 1);
        public abstract bool DeleteTask(int tid);
        public abstract void Rest();
        protected abstract int GenerateTid();

    }
}
