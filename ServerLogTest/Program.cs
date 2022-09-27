using PEUtils;
using PEUtilsTest;

internal class Program {

    private static void Main(string[] args) {
        //PELogTest test = new();
        //test.Test();

        PETimerTest pETimer = new();
        //pETimer.TickTimerTest();
        //pETimer.TickTimerTestHandle();
        //pETimer.TickTimerTestUpdate();
        pETimer.TickTimerTestUpdateHandle();
    }
}