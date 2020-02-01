// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WebRTC.iOS.Demo
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		UIKit.UIButton AppRTCButton { get; set; }

		[Outlet]
		UIKit.UIButton H113Button { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AppRTCButton != null) {
				AppRTCButton.Dispose ();
				AppRTCButton = null;
			}

			if (H113Button != null) {
				H113Button.Dispose ();
				H113Button = null;
			}
		}
	}
}
