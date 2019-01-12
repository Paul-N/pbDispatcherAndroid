using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class CountersController : UIViewController
    {
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;

        Sensor sensor = new Sensor();
        Dictionary<byte, double> nodeValues = new Dictionary<byte, double>();

        public CountersController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = "Настройки счетчика";

            SetLayout();

            if ((Application.selectedMachine != null) && (Application.selectedMachine.sensors.Count != 0))
                sensor = Application.selectedMachine.sensors[0];

            DataManager.SheduleGetNodeValuesRequest(sensor.nodeID, DataUpdateCallback);

            if ((sensor.type.code == SensorTypeCodes.Pulse) || (sensor.type.code == SensorTypeCodes.IRDAModem))
            {
                Setting1Label.Text = "T1";
                Setting2Label.Text = "T2";
                Setting3Label.Text = "Имп - 1 кВт/ч:";
                SettingsTotalLabel.Hidden = false;
            }
            else if (sensor.type.code == SensorTypeCodes.Pulse2Channel)
            {
                Setting1Label.Text = "Канал 1:";
                Setting2Label.Text = "Канал 2:";
                Setting3Label.Text = "Имп - м3:";
                SettingsTotalLabel.Hidden = true;
            }

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
            if ((requestResult is Dictionary<byte, double>) == false)
                return;

            nodeValues = (Dictionary<byte, double>)requestResult;

            needDataUpdate++;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            Title = ViewUtils.FormTitle("Настройки счетчика");
            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues()
        {
            InventoryIDLabel.Text = "Инв. №: " + Application.selectedMachine.inventoryID;
            if (Application.selectedMachine.sensors.Count != 0)
                InventoryIDLabel.Text += " (" + Application.selectedMachine.sensors[0].nodeID + ")";

            MachineIcon.Image = UIImage.FromFile(Application.selectedMachine.type.iconName);
            MachineNameLabel.Text = Application.selectedMachine.GetNameStr();
            DivisionLabel.Text = Application.selectedMachine.GetDivisionStr();

            var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
            numberFormatInfo.NumberGroupSeparator = " ";
            numberFormatInfo.NumberDecimalSeparator = ",";

            double val;
            if ((sensor.type.code == SensorTypeCodes.Pulse) || (sensor.type.code == SensorTypeCodes.IRDAModem))
            {
                val = 0;
                nodeValues.TryGetValue (2, out val);
                Setting1Field.Text = val.ToString("N", numberFormatInfo);

                val = 0;
                nodeValues.TryGetValue(1, out val);
                Setting2Field.Text = val.ToString("N", numberFormatInfo);

                val = 0;
                nodeValues.TryGetValue(0, out val);
                SettingsTotalLabel.Text = val.ToString("N", numberFormatInfo);

                val = 0;
                nodeValues.TryGetValue(3, out val);
                Setting3Field.Text = val.ToString("N", numberFormatInfo);
            }
            else if (sensor.type.code == SensorTypeCodes.Pulse2Channel)
            {
                val = 0;
                nodeValues.TryGetValue(0, out val);
                Setting1Field.Text = val.ToString("N", numberFormatInfo);

                val = 0;
                nodeValues.TryGetValue(2, out val);
                Setting2Field.Text = val.ToString("N", numberFormatInfo);

                val = 0;
                nodeValues.TryGetValue(1, out val);
                Setting3Field.Text = val.ToString("N", numberFormatInfo);
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void OkButton_TouchUpInside(UIButton sender)
        {
            try 
            {
                double val;

                if ((sensor.type.code == SensorTypeCodes.Pulse) || (sensor.type.code == SensorTypeCodes.IRDAModem))
                {
                    if (double.TryParse(Setting1Field.Text.Replace(" ", "").Replace(".", ","), out val) == true)
                        nodeValues[2] = val;

                    if (double.TryParse(Setting2Field.Text.Replace(" ", "").Replace(".", ","), out val) == true)
                        nodeValues[1] = val;
                       
                    nodeValues[0] = nodeValues[1] + nodeValues[2];

                    if (double.TryParse(Setting3Field.Text.Replace(" ", "").Replace(".", ","), out val) == true)
                        nodeValues[3] = val;
                }
                else if (sensor.type.code == SensorTypeCodes.Pulse2Channel)
                {
                    if (double.TryParse(Setting1Field.Text.Replace(" ", "").Replace(".", ","), out val) == true)
                        nodeValues[0] = val;

                    if (double.TryParse(Setting2Field.Text.Replace(" ", "").Replace(".", ","), out val) == true)
                        nodeValues[2] = val;

                    if (double.TryParse(Setting3Field.Text.Replace(" ", "").Replace(".", ","), out val) == true)
                        nodeValues[1] = val;

                    nodeValues[3] = nodeValues[1];
                }
            } 
            catch {}

            ByteBuffer buffer = new ByteBuffer();
            foreach (KeyValuePair<byte, double> nodeValue in (Dictionary<byte, double>)nodeValues)
            {
                buffer.Add((byte)nodeValue.Key);
                buffer.Add((double)nodeValue.Value);
            }

            DataManager.SheduleSetNodeValuesRequest(sensor.nodeID, buffer, null);
            DataManager.SheduleGetNodeValuesRequest(sensor.nodeID, DataUpdateCallback);

            OkButton.SetTitle(OkButton.Title(UIControlState.Normal) + "+", UIControlState.Normal);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ChancelButton_TouchUpInside(UIButton sender)
        {
            NavigationController?.PopViewController(true);
        }

    }
}