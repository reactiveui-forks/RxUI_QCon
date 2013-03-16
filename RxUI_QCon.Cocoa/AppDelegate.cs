using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using ReactiveUI;

namespace RxUI_QCon.Cocoa
{
    public partial class AppDelegate : NSApplicationDelegate
    {
        MainWindowController mainWindowController;
        
        public AppDelegate()
        {
            // NB: Wherein I work around all of the RxUI bugs.
            RxApp.Register(typeof(DummyPropertyBindingHook), typeof(IPropertyBindingHook));
            (new ReactiveUI.Xaml.ServiceLocationRegistration()).Register();
            (new ReactiveUI.Routing.ServiceLocationRegistration()).Register();
            (new ReactiveUI.Cocoa.ServiceLocationRegistration()).Register();
        }

        public override void FinishedLaunching(NSObject notification)
        {
            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(this);
        }
    }

    public class DummyPropertyBindingHook : IPropertyBindingHook
    {
        public bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            return true;
        }
    }
}

