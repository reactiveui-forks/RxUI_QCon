using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using ReactiveUI;
using ReactiveUI.Xaml;
using MonoMac.CoreGraphics;
using System.Drawing;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace RxUI_QCon.Cocoa
{
    public partial class MainWindowController : MonoMac.AppKit.NSWindowController, IViewFor<MainWindowViewModel>, INotifyPropertyChanged
    {
        // Called when created from unmanaged code
        public MainWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public MainWindowController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public MainWindowController() : base ("MainWindow")
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
            ViewModel = new MainWindowViewModel();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.Bind(ViewModel, x => x.Red, x => x.redField.StringValue, hookTextField(redField));
            this.Bind(ViewModel, x => x.Green, x => x.greenField.StringValue, hookTextField(greenField));
            this.Bind(ViewModel, x => x.Blue, x => x.blueField.StringValue, hookTextField(blueField));

            // NB: Cocoa is Weird.
            finalColorView.WantsLayer = true;
            this.WhenAny(x => x.ViewModel.FinalColor, x => x.Value)
                .Where(x => x != null)
                .Select(x => new CALayer() { BackgroundColor = colorToCgColor(x.Value) })
                .BindTo(this, x => x.finalColorView.Layer);

            this.BindCommand(ViewModel, x => x.Ok, x => x.okButton);

            this.WhenAny(x => x.ViewModel.Images, x => x.Value)
                .Where(x => x != null)
                .Select(x => x.Cast<NSObject>().ToArray())
                .BindTo(this, x => x.collectionView.Content);

            this.OneWayBind(ViewModel, x => x.IsBusy, x => x.progressIndicator.Hidden, x => !x);
            progressIndicator.StartAnimation(this);
        }

        // NB: This design is terrible and leaks memory like a sieve. For example
        // purposes only!
        IObservable<Unit> hookTextField(NSTextField textField)
        {
            var ret = new Subject<Unit>();
            textField.Delegate = new BlockDidChangeTextFieldDelegate(() => ret.OnNext(Unit.Default));
            return ret;
        }

        class BlockDidChangeTextFieldDelegate : NSTextFieldDelegate
        {
            Action block;
            public BlockDidChangeTextFieldDelegate(Action block) { this.block = block; }
            public override void Changed(NSNotification notification) { block(); }
        }

        CGColor colorToCgColor(Color c)
        {
            float r = c.R / 255.0f, g = c.G / 255.0f, b = c.B / 255.0f, a = c.A / 255.0f;
            return new CGColor(r,g,b,a);
        }

        public MainWindowViewModel ViewModel { get; set; }
        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (MainWindowViewModel)value; }
        }

        public new MainWindow Window {
            get { return (MainWindow)base.Window; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [Register("JustDisplayTheImageCollectionViewItem")]
    public class JustDisplayTheImageCollectionViewItem : NSCollectionViewItem
    {
        // Called when created from unmanaged code
        public JustDisplayTheImageCollectionViewItem(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public JustDisplayTheImageCollectionViewItem(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public JustDisplayTheImageCollectionViewItem() : base ()
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
            RxApp.DeferredScheduler.Schedule(() => {
                if (this.RepresentedObject == null) return;
                this.View = new NSImageView() { 
                    Image = (NSImage)this.RepresentedObject,
                    ImageFrameStyle = NSImageFrameStyle.Photo,
                };
            });
        }
    }
}