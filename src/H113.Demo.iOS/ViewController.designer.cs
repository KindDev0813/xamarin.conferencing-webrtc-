// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace H113.Demo.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton CameraButton { get; set; }

		[Outlet]
		UIKit.UIButton DialButton { get; set; }

		[Outlet]
		UIKit.UIButton MicButton { get; set; }

		[Outlet]
		UIKit.UIStackView StackView { get; set; }

		[Outlet]
		UIKit.UIButton StartCallButton { get; set; }

		[Outlet]
		UIKit.UIView VideoContainer { get; set; }

		[Outlet]
		UIKit.UIView VideoControllerContainer { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CameraButton != null) {
				CameraButton.Dispose ();
				CameraButton = null;
			}

			if (MicButton != null) {
				MicButton.Dispose ();
				MicButton = null;
			}

			if (StackView != null) {
				StackView.Dispose ();
				StackView = null;
			}

			if (StartCallButton != null) {
				StartCallButton.Dispose ();
				StartCallButton = null;
			}

			if (VideoContainer != null) {
				VideoContainer.Dispose ();
				VideoContainer = null;
			}

			if (VideoControllerContainer != null) {
				VideoControllerContainer.Dispose ();
				VideoControllerContainer = null;
			}

			if (DialButton != null) {
				DialButton.Dispose ();
				DialButton = null;
			}

		}
	}
}
