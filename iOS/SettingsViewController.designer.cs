// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace pbDispatcher.iOS
{
    [Register ("SettingsViewController")]
    partial class SettingsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView FilterTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ServerButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton UserButton { get; set; }

        [Action ("ServerButtonTouch:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ServerButtonTouch (UIKit.UIButton sender);

        [Action ("UserButtonTouch:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UserButtonTouch (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (FilterTable != null) {
                FilterTable.Dispose ();
                FilterTable = null;
            }

            if (ServerButton != null) {
                ServerButton.Dispose ();
                ServerButton = null;
            }

            if (UserButton != null) {
                UserButton.Dispose ();
                UserButton = null;
            }
        }
    }
}