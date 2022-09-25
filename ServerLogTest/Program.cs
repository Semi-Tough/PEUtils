using PEUtils;

internal class Program {

    class Test1 {
        public void Init() {
            Test2 test = new Test2();
            test.Init();
            this.Error("Test errot !");
        }
    }
    class Test2 {
        public void Init() {
            this.Error("Test errot !");
        }
    }


    private static void Main(string[] args) {
        PELog.InitSetting();


        PELog.ColorLog("Hello World !", LogColor.None);
        PELog.ColorLog("Hello World !", LogColor.Red);
        PELog.ColorLog("Hello World !", LogColor.Green);
        PELog.ColorLog("Hello World !", LogColor.Blue);
        PELog.ColorLog("Hello World !", LogColor.Cyan);
        PELog.ColorLog("Hello World !", LogColor.Magenta);
        PELog.ColorLog("Hello World !", LogColor.Yellow);


        PELog.Log("Hello World !");
        PELog.ColorLog("Hello World !", LogColor.Cyan);
        PELog.Wain("Hello World !");
        PELog.Error("Hello World !");
        PELog.Trace("Hello World !");

        object obj = "2134";
        PELog.Log(obj);
        PELog.ColorLog(obj, LogColor.Cyan);
        PELog.Wain(obj);
        PELog.Error(obj);
        PELog.Trace(obj);

        object obj2 = new();
        obj2.Log(obj2);
        obj2.ColorLog(obj2, LogColor.Green);
        obj2.Wain(obj2);
        obj2.Error(obj2);
        obj2.Trace(obj2);

        obj2.Log("log");
        obj2.ColorLog("ColorLog", LogColor.Green);
        obj2.Wain("Wain");
        obj2.Error("Error");
        obj2.Trace("Trace");

        Test1 test = new();
        test.Init();
    
    
    }
}