using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;
using CoreGraphics;

namespace pbDispatcher.iOS
{
    public partial class MachineSensorsController : UIViewController
    {
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;
        string titleStr = "Датчики оборудования";

        List<Sensor> attachedSensors = new List<Sensor>();
        List<Sensor> availableSensors = new List<Sensor>();

        public MachineSensorsController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            titleStr = Application.selectedMachine.GetNameStr();


            AttachedSensorsTable.Source = new AttachedSensorsTableSource(attachedSensors, OnAttachedRowSelected);
            AvailableSensorsTable.Source = new AvailableSensorsTableSource(availableSensors, OnAvailableRowSelected);

            AttachedSensorsTable.Layer.BorderColor = UIColor.LightGray.CGColor;
            AttachedSensorsTable.Layer.BorderWidth = 1f;

            AvailableSensorsTable.Layer.BorderColor = UIColor.LightGray.CGColor;
            AvailableSensorsTable.Layer.BorderWidth = 1f;

            UpdateViewValues();

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = titleStr;

            SetLayout();

            DataManager.SheduleGetAvailableSensorsRequest(Application.selectedMachine.divisionOwner.ID, DataUpdateCallback);

            UpdateViewValues();

            Application.StartUpdateTimer(CheckNewData);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout()
        {
            ViewUtils.ExpandWidth(AttachedSensorsTable, 3);
            ViewUtils.ExpandWidth(AvailableSensorsTable, 3);
            ViewUtils.ExpandHeight(AvailableSensorsTable, 0);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestResult)
        {
            if (requestResult is List<Sensor>)
            {
                availableSensors.Clear();
                foreach (var item in (List<Sensor>)requestResult)
                    availableSensors.Add(item);

                for (int s = 0; s < DataManager.machines.Count; s++)
                    if (Application.selectedMachine.ID == DataManager.machines[s].ID)
                    {
                        Application.selectedMachine = DataManager.machines[s];

                        attachedSensors.Clear();
                        foreach (var item in Application.selectedMachine.sensors)
                            attachedSensors.Add(item);

                        break;
                    }
                needDataUpdate++;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            Title = ViewUtils.FormTitle(titleStr);
            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues()
        {
            AttachedSensorsTable.ReloadData();
            AvailableSensorsTable.ReloadData();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void OnAttachedRowSelected()
        {
            if (attachedSensors.Count <= AttachedSensorsTable.IndexPathForSelectedRow.Row)
                return;

            Sensor sensor = attachedSensors[AttachedSensorsTable.IndexPathForSelectedRow.Row];
            string str = "Открепить датчик " + sensor.nodeID + " - " + sensor.ID + "\n" + "от " + Application.selectedMachine.GetNameStr() + "?";

            UIAlertController alert = UIAlertController.Create("Открепление", str, UIAlertControllerStyle.Alert);

            // Configure the alert
            alert.AddAction(UIAlertAction.Create("Да", UIAlertActionStyle.Default, (action) => {
                DataManager.SheduleDetachSensorRequest(Application.selectedMachine.ID, sensor.ID, null);
                DataManager.SheduleGetMachinesRequest(null);
                DataManager.SheduleGetAvailableSensorsRequest(Application.selectedMachine.divisionOwner.ID, DataUpdateCallback);
            }));
            alert.AddAction(UIAlertAction.Create("Отмена", UIAlertActionStyle.Default, (action) => { }));

            // Display the alert
            PresentViewController(alert, true, null);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void OnAvailableRowSelected()
        {
            if (availableSensors.Count <= AvailableSensorsTable.IndexPathForSelectedRow.Row)
                return;

            Sensor sensor = availableSensors[AvailableSensorsTable.IndexPathForSelectedRow.Row];
            string str = "Прикрепить датчик " + sensor.nodeID + " - " + sensor.ID + "\n" + "к " + Application.selectedMachine.GetNameStr() + "?";

            UIAlertController alert = UIAlertController.Create("Прикрепление", str, UIAlertControllerStyle.Alert);

            // Configure the alert
            alert.AddAction(UIAlertAction.Create("Да", UIAlertActionStyle.Default, (action) => {
                DataManager.SheduleAttachSensorRequest(Application.selectedMachine.ID, sensor.ID, null);
                DataManager.SheduleGetMachinesRequest(null);
                DataManager.SheduleGetAvailableSensorsRequest(Application.selectedMachine.divisionOwner.ID, DataUpdateCallback);
            }));
            alert.AddAction(UIAlertAction.Create("Отмена", UIAlertActionStyle.Default, (action) => { }));

            // Display the alert
            PresentViewController(alert, true, null);
        }

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class AttachedSensorsTableSource : UITableViewSource
    {
        string cellIdentifier = "AttachedSensorsCell";

        public List<Sensor> dataSource;
        Action onRowSelected;

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public AttachedSensorsTableSource(List<Sensor> dataSource, Action onRowSelected)
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
            var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as AttachedSensorsTableCell;

            if (indexPath.Row < dataSource.Count)
            {
                var item = dataSource[(ushort)indexPath.Row];

                cell.Icon = item.type.iconName;
                cell.Name = item.nodeID.ToString();
                cell.Comment = item.lastTime.ToString("dd.MM.yy  HH:mm:ss") + ", " + item.ID + ", " + item.rssi + ", " + item.battery + "%, " + item.chipTemperature + "°";

                cell.MainValue = item.mainValue.ToString("F2");

                var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
                numberFormatInfo.NumberGroupSeparator = " ";
                numberFormatInfo.NumberDecimalSeparator = ",";

                cell.AdditionalValue = item.additionalValue.ToString("N", numberFormatInfo);

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

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class AvailableSensorsTableSource : UITableViewSource
    {
        string cellIdentifier = "AvailableSensorsCell";

        public List<Sensor> dataSource;
        Action onRowSelected;

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public AvailableSensorsTableSource(List<Sensor> dataSource, Action onRowSelected)
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
            var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as AvailableSensorsTableCell;

            if (indexPath.Row < dataSource.Count)
            {
                var item = dataSource[(ushort)indexPath.Row];

                cell.Icon = item.type.iconName;
                cell.Name = item.nodeID.ToString();
                cell.Comment = item.lastTime.ToString("dd.MM.yy  HH:mm:ss") + ", " + item.ID + ", " + item.rssi + ", " + item.battery + "%, " + item.chipTemperature + "°";

                cell.MainValue = item.mainValue.ToString("F2");

                var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
                numberFormatInfo.NumberGroupSeparator = " ";
                numberFormatInfo.NumberDecimalSeparator = ",";

                cell.AdditionalValue = item.additionalValue.ToString("N", numberFormatInfo);

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