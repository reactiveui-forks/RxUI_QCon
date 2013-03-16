using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace RxUI_QCon.Cocoa
{
    public partial class AppDelegate : NSApplicationDelegate
    {
        MainWindowController mainWindowController;
        
        public AppDelegate()
        {
        }

        public override void FinishedLaunching(NSObject notification)
        {
            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(this);
        }
    }
}

