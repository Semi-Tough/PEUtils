PEUtils.PELog.InitSetting();

//PEUtils.PELog.Log("Hello World !");
//PEUtils.PELog.ColorLog("Hello World !",PEUtils.LogColor.Cyan);
//PEUtils.PELog.Wain("Hello World !");
//PEUtils.PELog.Error("Hello World !");
//PEUtils.PELog.Trace("Hello World !");

//object obj= "2134";
//PEUtils.PELog.Log(obj);
//PEUtils.PELog.ColorLog(obj, PEUtils.LogColor.Cyan);
//PEUtils.PELog.Wain(obj);
//PEUtils.PELog.Error(obj);
//PEUtils.PELog.Trace(obj);

object obj2= new();
obj2.Log(obj2);
obj2.ColorLog(obj2, PEUtils.LogColor.Green);
obj2.Wain(obj2);
obj2.Error(obj2);
obj2.Trace(obj2);

obj2.Log("log");
obj2.ColorLog("ColorLog", PEUtils.LogColor.Green);
obj2.Wain("Wain");
obj2.Error("Error");
obj2.Trace("Trace");


