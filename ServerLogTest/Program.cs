using PEUtils;

internal class Program {

    private static void Main(string[] args) {
        //PELogTest test = new();
        //test.Test();

        PETimerTest pETimer = new();
        //pETimer.TickTimerTest();
        //pETimer.TickTimerTestHandle();
        //pETimer.TickTimerTestUpdate();
        //pETimer.TickTimerTestUpdateHandle();
        pETimer.AsyncTimerTest();
        //pETimer.AsyncTimerTestHandle();
    }
}