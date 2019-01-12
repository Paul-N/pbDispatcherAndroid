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
    [Register ("MachineSensorsController")]
    partial class MachineSensorsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView AttachedSensorsTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AvailableLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView AvailableSensorsTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AttachedSensorsTable != null) {
                AttachedSensorsTable.Dispose ();
                AttachedSensorsTable = null;
            }

            if (AvailableLabel != null) {
                AvailableLabel.Dispose ();
                AvailableLabel = null;
            }

            if (AvailableSensorsTable != null) {
                AvailableSensorsTable.Dispose ();
                AvailableSensorsTable = null;
            }
        }
    }
}