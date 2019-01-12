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
    [Register ("MachineStatesLogTableCell")]
    partial class MachineStatesLogTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CommentCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView IconCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PeriodCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PeriodIconCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TimeCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserCell { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CommentCell != null) {
                CommentCell.Dispose ();
                CommentCell = null;
            }

            if (IconCell != null) {
                IconCell.Dispose ();
                IconCell = null;
            }

            if (PeriodCell != null) {
                PeriodCell.Dispose ();
                PeriodCell = null;
            }

            if (PeriodIconCell != null) {
                PeriodIconCell.Dispose ();
                PeriodIconCell = null;
            }

            if (TimeCell != null) {
                TimeCell.Dispose ();
                TimeCell = null;
            }

            if (UserCell != null) {
                UserCell.Dispose ();
                UserCell = null;
            }
        }
    }
}