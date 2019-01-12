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
    [Register ("ServiceController")]
    partial class ServiceController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView CommentField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ConfirmButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel InventoryIDLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RejectButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StateCommentLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView StateIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StateTimeStartLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StateUserLabel { get; set; }

        [Action ("ConfirmButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ConfirmButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("RejectButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RejectButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CommentField != null) {
                CommentField.Dispose ();
                CommentField = null;
            }

            if (ConfirmButton != null) {
                ConfirmButton.Dispose ();
                ConfirmButton = null;
            }

            if (InventoryIDLabel != null) {
                InventoryIDLabel.Dispose ();
                InventoryIDLabel = null;
            }

            if (RejectButton != null) {
                RejectButton.Dispose ();
                RejectButton = null;
            }

            if (StateCommentLabel != null) {
                StateCommentLabel.Dispose ();
                StateCommentLabel = null;
            }

            if (StateIcon != null) {
                StateIcon.Dispose ();
                StateIcon = null;
            }

            if (StateTimeStartLabel != null) {
                StateTimeStartLabel.Dispose ();
                StateTimeStartLabel = null;
            }

            if (StateUserLabel != null) {
                StateUserLabel.Dispose ();
                StateUserLabel = null;
            }
        }
    }
}