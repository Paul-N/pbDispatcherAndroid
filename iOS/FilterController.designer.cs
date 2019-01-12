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
    [Register ("FilterController")]
    partial class FilterController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView FilterTable { get; set; }

        [Action ("UserButtonTouch:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UserButtonTouch (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (FilterTable != null) {
                FilterTable.Dispose ();
                FilterTable = null;
            }
        }
    }
}