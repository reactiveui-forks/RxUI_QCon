// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace RxUI_QCon.Cocoa
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField redField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField greenField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField blueField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView finalColorView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton okButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSCollectionView collectionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (redField != null) {
				redField.Dispose ();
				redField = null;
			}

			if (greenField != null) {
				greenField.Dispose ();
				greenField = null;
			}

			if (blueField != null) {
				blueField.Dispose ();
				blueField = null;
			}

			if (finalColorView != null) {
				finalColorView.Dispose ();
				finalColorView = null;
			}

			if (okButton != null) {
				okButton.Dispose ();
				okButton = null;
			}

			if (collectionView != null) {
				collectionView.Dispose ();
				collectionView = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
