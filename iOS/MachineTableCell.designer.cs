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
    [Register ("MachineTableCell")]
    partial class MachineTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AdditionalValueCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AdditionalValueSymbolCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DivisionCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView MachineIconCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView MachineStateIconCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MainValueCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MainValueSymbolCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NameCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView SensorIconCell { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AdditionalValueCell != null) {
                AdditionalValueCell.Dispose ();
                AdditionalValueCell = null;
            }

            if (AdditionalValueSymbolCell != null) {
                AdditionalValueSymbolCell.Dispose ();
                AdditionalValueSymbolCell = null;
            }

            if (DivisionCell != null) {
                DivisionCell.Dispose ();
                DivisionCell = null;
            }

            if (MachineIconCell != null) {
                MachineIconCell.Dispose ();
                MachineIconCell = null;
            }

            if (MachineStateIconCell != null) {
                MachineStateIconCell.Dispose ();
                MachineStateIconCell = null;
            }

            if (MainValueCell != null) {
                MainValueCell.Dispose ();
                MainValueCell = null;
            }

            if (MainValueSymbolCell != null) {
                MainValueSymbolCell.Dispose ();
                MainValueSymbolCell = null;
            }

            if (NameCell != null) {
                NameCell.Dispose ();
                NameCell = null;
            }

            if (SensorIconCell != null) {
                SensorIconCell.Dispose ();
                SensorIconCell = null;
            }
        }
    }
}