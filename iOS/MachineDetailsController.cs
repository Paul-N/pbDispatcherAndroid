using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class MachineDetailsController : UIViewController
    {
        private byte needDataUpdate = 0;
        private bool needHistoryGraphRedraw = false;
        DateTime lastUpdateTime = DateTime.MinValue;
        DateTime lastHistoryUpdateTime = DateTime.MinValue;

        private List<MachineStatesLogElement> machineStatesLogList = new List<MachineStatesLogElement>();
        private List<HistoryPoint> sensorHistoryList = new List<HistoryPoint>();
        private DateTime sensorHistoryTimeStart, sensorHistoryTimeEnd;

        public MachineDetailsController(IntPtr handle) : base(handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            MachineStatesLogTable.Source = new MachineStatesLogTableSource(machineStatesLogList, OnRowSelected);

            MachineStatesLogTable.Layer.BorderColor = UIColor.LightGray.CGColor;
            MachineStatesLogTable.Layer.BorderWidth = 1f;

            UpdateViewValues();

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            Title = Application.selectedMachine.name;

            base.ViewWillAppear(animated);

            UpdateViewValues();

            SetLayout();

            DataManager.SheduleGetMachineStatesLogRequest(Application.selectedMachine, Settings.machineStatesLogMaxElements, DataUpdateCallback);
            lastUpdateTime = DateTime.Now;

            sensorHistoryTimeStart = DateTime.Now.AddDays(-1);
            sensorHistoryTimeEnd = DateTime.Now;

            HistoryGraph.SetData(sensorHistoryList, sensorHistoryTimeStart, sensorHistoryTimeEnd);

            if (Application.selectedMachine.sensors.Count != 0)
                DataManager.SheduleGetSensorHistoryDataRequest(Application.selectedMachine.sensors[0], (byte)SensorValueArrayIndexes.MainValue, sensorHistoryTimeStart, sensorHistoryTimeEnd, Settings.sensorHistoryPointsCount, DataUpdateCallback);

            Application.StartUpdateTimer(CheckNewData);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            SetLayout();

            base.WillRotate(toInterfaceOrientation, duration);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout()
        {
            ViewUtils.ExpandWidth(InventoryIDLabel, 3);
            ViewUtils.ExpandWidth(MachineStatesLogTable, 3);

            ViewUtils.ExpandWidth(HistoryGraph, 3);
            ViewUtils.ExpandHeight(HistoryGraph, 3);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestResult)
        {
            if (requestResult is List<MachineStatesLogElement>)
            {
                machineStatesLogList.Clear();
                foreach (var item in (List<MachineStatesLogElement>)requestResult)
                {
                    machineStatesLogList.Add(item);
                    if ((item.timeEnd == DateTime.MinValue) && (item.state is MachineServiceState))
                    {
                        Application.selectedMachine.serviceState = (MachineServiceState)item.state;
                        Application.selectedMachine.serviceStateTimeStart = item.timeStart;
                        Application.selectedMachine.userName = item.userName;
                    }
                }

                needDataUpdate++;
            }

            if (requestResult is List<HistoryPoint>)
            {
                sensorHistoryList.Clear();
                foreach (var item in (List<HistoryPoint>)requestResult)
                {
                    sensorHistoryList.Add(item);
                }

                needHistoryGraphRedraw = true;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
            {
                Title = Application.selectedMachine.name;
            }
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                Title = "Нет авторизации";
            else
                Title = "Нет связи";

            if ((HistoryGraph.readyToDataUpdate == true) && ((sensorHistoryTimeStart != HistoryGraph.timeStart) || (sensorHistoryTimeEnd != HistoryGraph.timeEnd)))
            {
                HistoryGraph.readyToDataUpdate = false;

                sensorHistoryTimeStart = HistoryGraph.timeStart;
                sensorHistoryTimeEnd = HistoryGraph.timeEnd;

                if (Application.selectedMachine.sensors.Count != 0)
                    DataManager.SheduleGetSensorHistoryDataRequest(Application.selectedMachine.sensors[0], (byte)SensorValueArrayIndexes.MainValue, sensorHistoryTimeStart, sensorHistoryTimeEnd, Settings.sensorHistoryPointsCount, DataUpdateCallback);
            }

            if (needHistoryGraphRedraw == true)
            {
                needHistoryGraphRedraw = false;
                HistoryGraph.SetNeedsDisplay();
            }

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
            else if ((DateTime.Now.Subtract(lastUpdateTime).TotalMilliseconds > Settings.updatePeriodMs) && (DataManager.machineTypes.Count != 0) && (DataManager.NotAnsweredRequestsCount == 0))
            {
                DataManager.SheduleGetMachineStatesLogRequest(Application.selectedMachine, Settings.machineStatesLogMaxElements, DataUpdateCallback);

                lastUpdateTime = DateTime.Now;
            }

        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateViewValues()
        {
            InventoryIDLabel.Text = "Инв. №: " + Application.selectedMachine.inventoryID;
            if (Application.selectedMachine.sensors.Count != 0)
            {
                //                InventoryIDLabel.Text += " (" + BitConverter.ToString(BitConverter.GetBytes(Application.selectedMachine.sensors[0].nodeID), 1, 1)
                //                        + BitConverter.ToString(BitConverter.GetBytes(Application.selectedMachine.sensors[0].nodeID), 0, 1) + ")";
                InventoryIDLabel.Text += " (" + Application.selectedMachine.sensors[0].nodeID + ")";
            }
            MachineStatesLogTable.ReloadData();

            HistoryGraph.SetNeedsDisplay();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void OnRowSelected()
        {
            if (machineStatesLogList.Count <= MachineStatesLogTable.IndexPathForSelectedRow.Row)
                return;

            Application.StopUpdateTimer();
            MachineStatesLogElement element = machineStatesLogList[MachineStatesLogTable.IndexPathForSelectedRow.Row];

            if (Application.selectedMachine.camBridgeMac != 0)
            { 
                CamController.selectedCamTime = element.timeStart;
                NavigationController?.PushViewController(Storyboard?.InstantiateViewController("CamController"), true);
            }
            else
            {
                string str = element.description + "\n\n" + element.timeStart;
                if (element.timeEnd != DateTime.MinValue)
                    str += " - \n" + element.timeEnd;

                str += "\n" + element.userName;

                UIAlertController alert = UIAlertController.Create(element.state.name, str, UIAlertControllerStyle.Alert);

                // Configure the alert
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => { Application.StartUpdateTimer(CheckNewData); }));

                // Display the alert
                PresentViewController(alert, true, null);
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ServiceButton_TouchUpInside(UIButton sender)
        {
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("ServiceController"), true);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void SensorSettingsButon_TouchUpInside(UIButton sender)
        {
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("SensorSettingsController"), true);
        }
    }


    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachineStatesLogTableSource : UITableViewSource
    {
        string cellIdentifier = "MachineStatesLogCell";

        public List<MachineStatesLogElement> dataSource;
        Action onRowSelected;

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public MachineStatesLogTableSource(List<MachineStatesLogElement> dataSource, Action onRowSelected)
        {
            this.dataSource = dataSource;
            this.onRowSelected = onRowSelected;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return dataSource.Count;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as MachineStatesLogTableCell;

            if (indexPath.Row < dataSource.Count)
            {
                var item = dataSource[(ushort)indexPath.Row];

                cell.Icon = item.state.iconName;
                if ((item.description != null) && item.description.Length != 0)
                    cell.Comment = item.description;
                else
                    cell.Comment = item.state.name;
                
                cell.Time = item.timeStart.ToString ("dd.MM.yy  HH:mm:ss");
                if (item.timeEnd > item.timeStart)
                    cell.Period = FormatUtils.PeriodStr(item.timeStart, item.timeEnd);
                else
                    cell.Period = null;

                cell.User = item.userName;

                cell.UserInteractionEnabled = true;

                cell.SetLayout();
            }

            return cell;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            onRowSelected?.Invoke();
        }
    }

}