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
    [Register ("MachineDetailsController")]
    partial class MachineDetailsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        pbDispatcher.iOS.HistoryGraphView HistoryGraph { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel InventoryIDLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView MachineStatesLogTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SensorSettingsButon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ServiceButton { get; set; }

        [Action ("SensorSettingsButon_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SensorSettingsButon_TouchUpInside (UIKit.UIButton sender);

        [Action ("ServiceButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ServiceButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (HistoryGraph != null) {
                HistoryGraph.Dispose ();
                HistoryGraph = null;
            }

            if (InventoryIDLabel != null) {
                InventoryIDLabel.Dispose ();
                InventoryIDLabel = null;
            }

            if (MachineStatesLogTable != null) {
                MachineStatesLogTable.Dispose ();
                MachineStatesLogTable = null;
            }

            if (SensorSettingsButon != null) {
                SensorSettingsButon.Dispose ();
                SensorSettingsButon = null;
            }

            if (ServiceButton != null) {
                ServiceButton.Dispose ();
                ServiceButton = null;
            }
        }
    }
}