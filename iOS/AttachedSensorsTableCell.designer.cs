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
    [Register ("AttachedSensorsTableCell")]
    partial class AttachedSensorsTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AdditionalValueCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CommentCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView IconCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MainValueCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NameCell { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AdditionalValueCell != null) {
                AdditionalValueCell.Dispose ();
                AdditionalValueCell = null;
            }

            if (CommentCell != null) {
                CommentCell.Dispose ();
                CommentCell = null;
            }

            if (IconCell != null) {
                IconCell.Dispose ();
                IconCell = null;
            }

            if (MainValueCell != null) {
                MainValueCell.Dispose ();
                MainValueCell = null;
            }

            if (NameCell != null) {
                NameCell.Dispose ();
                NameCell = null;
            }
        }
    }
}