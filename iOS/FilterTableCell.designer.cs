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
    [Register ("FilterTableCell")]
    partial class FilterTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CityCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView IconCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NameCell { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CityCell != null) {
                CityCell.Dispose ();
                CityCell = null;
            }

            if (IconCell != null) {
                IconCell.Dispose ();
                IconCell = null;
            }

            if (NameCell != null) {
                NameCell.Dispose ();
                NameCell = null;
            }
        }
    }
}