using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace RxUI_QCon.Cocoa
{
    class MainClass
    {
        static void Main(string [] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}    

