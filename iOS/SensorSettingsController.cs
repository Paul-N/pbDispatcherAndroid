using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class SensorSettingsController : UIViewController
    {
        public int closeViewCounter = -1;
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;

        Sensor sensor = new Sensor();
        SensorBordersList sensorBorders = new SensorBordersList();
        SensorBorder sensorBorder = new SensorBorder();

        public SensorSettingsController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = "Тревоги датчика";
            sensorBorders = new SensorBordersList();
            sensorBorder = new SensorBorder();

            SetLayout();

            if ((Application.selectedMachine != null) && (Application.selectedMachine.sensors.Count != 0))
                sensor = Application.selectedMachine.sensors[0];

            DataManager.SheduleGetSensorBordersRequest(sensor, DataUpdateCallback);
            SensorIcon.Hidden = true;

            UpdateViewValues();

            Application.StartUpdateTimer(CheckNewData);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout()
        {
            ViewUtils.ExpandWidth(InventoryIDLabel, 3);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestResult)
        {
            if ((requestResult is SensorBordersList) == false)
                return;

            sensorBorder = new SensorBorder();

            if (((SensorBordersList)requestResult).list.Count == 0)
            {
                if (sensor.ID != 0)
                {
                    sensorBorders = new SensorBordersList();
                    sensorBorders.maxSecondsNotOkValue = 1;
                    sensorBorders.list.Add(new SensorBorder(SensorBorderTypes.Ok, 0, 0));
                    sensorBorder = sensorBorders.list[0];

                    needDataUpdate++;
                }
                return;
            }

            sensorBorders = (SensorBordersList)requestResult;

            foreach (var item in sensorBorders.list)
            {
                if (item.type == SensorBorderTypes.Ok)
                {
                    sensorBorder = item;
                    break;
                }
            }

            needDataUpdate++;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                Title = "Тревоги датчика";
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                Title = "Нет авторизации";
            else
                Title = "Нет связи";

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }

            if (closeViewCounter > 0)
                closeViewCounter--;
            else if ((closeViewCounter == 0) && (NavigationController != null) && (NavigationController.TopViewController == this))
                NavigationController.PopViewController(true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues()
        {
            InventoryIDLabel.Text = "Инв. №: " + Application.selectedMachine.inventoryID;
            if (Application.selectedMachine.sensors.Count != 0)
                InventoryIDLabel.Text += " (" + Application.selectedMachine.sensors[0].nodeID + ")";

            MachineIcon.Image = UIImage.FromFile(Application.selectedMachine.type.iconName);
            SensorIcon.Image = UIImage.FromFile(sensor.type.iconName);
            MachineNameLabel.Text = Application.selectedMachine.GetNameStr();
            DivisionLabel.Text    = Application.selectedMachine.GetDivisionStr();

            var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
            numberFormatInfo.NumberGroupSeparator = " ";
            numberFormatInfo.NumberDecimalSeparator = ",";

            BorderFromField.Text = sensorBorder.minValue.ToString("N", numberFormatInfo);
            BorderToField.Text = sensorBorder.maxValue.ToString("N", numberFormatInfo);

            ValueSymbolFromLabel.Text = sensor.type.mainValueSymbol;
            ValueSymbolToLabel.Text   = sensor.type.mainValueSymbol;

            int seconds = sensorBorders.maxSecondsNotOkValue;
            int hours = seconds / 3600;
            seconds -= hours * 3600;
            int minutes = seconds / 60;
            seconds -= minutes * 60;

            HoursField.Text   = hours.ToString();
            MinutesField.Text = minutes.ToString("D2");
            SecondsField.Text = seconds.ToString("D2");

            SensorIcon.Hidden = false;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void OkButton_TouchUpInside(UIButton sender)
        {
            ByteBuffer buffer = new ByteBuffer();

            double value;
            if (double.TryParse(BorderFromField.Text.Replace(" ", "").Replace(".", ","), out value) == true)
                sensorBorder.minValue = value;
            if (double.TryParse(BorderToField.Text.Replace(" ", "").Replace(".", ","), out value) == true)
                sensorBorder.maxValue = value;

            int hours, minutes, seconds;
            if ((int.TryParse(HoursField.Text, out hours) == true) && (int.TryParse(MinutesField.Text, out minutes) == true) && (int.TryParse(SecondsField.Text, out seconds) == true))
                sensorBorders.maxSecondsNotOkValue = hours * 3600 + minutes * 60 + seconds;

            DataManager.SheduleSetSensorBordersRequest(sensor, sensorBorders, DataUpdateCallback);
            DataManager.SheduleGetSensorBordersRequest(sensor, DataUpdateCallback);

            SensorIcon.Hidden = true;

            closeViewCounter = 5;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ChancelButton_TouchUpInside(UIButton sender)
        {
            NavigationController?.PopViewController(true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void CountersButton_TouchUpInside(UIButton sender)
        {
            if ((sensor.type.code == SensorTypeCodes.Pulse) || (sensor.type.code == SensorTypeCodes.IRDAModem) || (sensor.type.code == SensorTypeCodes.Pulse2Channel))
                NavigationController?.PushViewController(Storyboard?.InstantiateViewController("CountersController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void PhotoButton_TouchUpInside(UIButton sender)
        {
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("PhotoController"), true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void SensorsButton_TouchUpInside(UIButton sender)
        {
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("MachineSensorsController"), true);
        }
    }
}