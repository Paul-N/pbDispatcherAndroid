using Foundation;
using System;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class MachinesController : UITableViewController
    {
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;
        UIImageView pingImageView = new UIImageView();
        UILabel machinesTitleLabel = new UILabel();
        UIView titleView = new UIView();

        public MachinesController(IntPtr handle) : base(handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MachineTable.Source = new MachinesTableSource(this);
            UpdateViewValues();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //machinesTitleLabel.Text = "Проспект Мира 72/2 1234567890123456789012345678901234567890";
            machinesTitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
            machinesTitleLabel.TextAlignment = UITextAlignment.Center;
            titleView.AddSubview(pingImageView);
            titleView.AddSubview(machinesTitleLabel);

            MachinesTitle.TitleView = titleView;

            SetLayout();

            DataManager.SheduleGetMachinesRequest(null);
            DataManager.SheduleGetCamsRequest(DataUpdateCallback);
            lastUpdateTime = DateTime.Now;

            if (DataManager.LoginImpossible == true)
                NavigationController?.PushViewController(Storyboard?.InstantiateViewController("UserSettingsController"), true);

            Application.StartUpdateTimer(CheckNewData);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillDisappear (bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            SetLayout();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout()
        {
            titleView.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 100, 30);
            pingImageView.Frame = new CoreGraphics.CGRect(-6, 0, 5, 5);
            machinesTitleLabel.Frame = new CoreGraphics.CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 105, 30);

            MachineTable.ReloadData();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestState)
        {
            //if (requestState == RequestStates.Completed)
            needDataUpdate++;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
            {
                machinesTitleLabel.Text = "Все";

                if (DataManager.selectedDivision != null)
                    machinesTitleLabel.Text = DataManager.selectedDivision.name;

                if (DataManager.Ping < 200)
                    pingImageView.Image = UIImage.FromFile("GreenCircle.png");
                else if (DataManager.Ping < 500)
                    pingImageView.Image = UIImage.FromFile("YelowCircle.png");
                else
                    pingImageView.Image = UIImage.FromFile("RedCircle.png");
            }
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
            {
                machinesTitleLabel.Text = "Нет авторизации";
                pingImageView.Image = UIImage.FromFile("GreyCircle.png");
            }
            else
            {
                machinesTitleLabel.Text = "Нет связи";
                pingImageView.Image = UIImage.FromFile("GreyCircle.png");
            }

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
                pingImageView.Hidden = false;
            }
            else if ((DateTime.Now.Subtract(lastUpdateTime).TotalMilliseconds > Settings.updatePeriodMs) && (DataManager.machineTypes.Count != 0) && (DataManager.NotAnsweredRequestsCount == 0))
            {
                DataManager.SheduleGetMachinesRequest(null);
                DataManager.SheduleGetCamsRequest(DataUpdateCallback);
                lastUpdateTime = DateTime.Now;
                pingImageView.Hidden = true;
            }



        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void UpdateViewValues()
        {
            MachineTable.ReloadData();

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = DataManager.problemsCount;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void SettingsButton_TouchUpInside(UIButton sender)
        {
            NavigationController.PushViewController(Storyboard.InstantiateViewController("FiltersListController"), false);
        }
    }


    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public class MachinesTableSource : UITableViewSource
    {
        string cellIdentifier = "MachineCell";
        MachinesController parentController;

        public MachinesTableSource(MachinesController parentController)
        {
            this.parentController = parentController;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return DataManager.machines.Count;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as MachineTableCell;

            if (indexPath.Row < DataManager.machines.Count)
            {
                var item = DataManager.machines[indexPath.Row];

                cell.MachineIcon = item.type.iconName;

                if (item.serviceState.code > MachineServiceStateCodes.Work)
                    cell.MachineStateIcon = item.serviceState.iconName;
                else
                    cell.MachineStateIcon = item.state.iconName;

                cell.Name = item.GetNameStr();

                cell.Division = item.GetDivisionStr();

                if (item.sensors.Count != 0)
                {
                    if (item.type.code == MachineTypeCodes.Oven)
                    {
                        Sensor sensor = item.sensors[0];

                        cell.SensorIcon = sensor.type.iconName;
                        cell.MainValue = sensor.mainValue.ToString("F0");
                        cell.MainValueSymbol = sensor.type.mainValueSymbol;

                        cell.AdditionalValueSymbol = sensor.type.additionalValueSymbol;

                        if (DataManager.userSettings.role == UserRoles.Admin)
                        {
                            if (DataManager.selectedDivision != null)
                                cell.Division = item.inventoryID + "  (" + sensor.nodeID + ", " + sensor.rssi + ", " + sensor.chipTemperature + "°)";
                            else
                                cell.Division = "(" + sensor.nodeID + ", " + sensor.rssi + ", " + sensor.chipTemperature + "°) " + cell.Division;
                        }

                        cell.AdditionalValue = "";
                        for (int s = 1; s < item.sensors.Count; s++)
                        {
                            if (s != 1)
                                cell.AdditionalValue += "° ";

                            cell.AdditionalValue += item.sensors[s].mainValue.ToString("F0");
                        }


                    }
                    else
                    {
                        Sensor sensor = item.sensors[0];

                        cell.SensorIcon = sensor.type.iconName;
                        cell.MainValue = sensor.mainValue.ToString("F2");
                        cell.MainValueSymbol = sensor.type.mainValueSymbol;

                        var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
                        numberFormatInfo.NumberGroupSeparator = " ";
                        numberFormatInfo.NumberDecimalSeparator = ",";

                        cell.AdditionalValue = sensor.additionalValue.ToString("N", numberFormatInfo);
                        cell.AdditionalValueSymbol = sensor.type.additionalValueSymbol;

                        if (DataManager.userSettings.role == UserRoles.Admin)
                        {
                            if (DataManager.selectedDivision != null)
                                cell.Division = item.inventoryID + "  (" + sensor.nodeID + ", " + sensor.rssi + ", " + sensor.chipTemperature + "°)";
                            else
                                cell.Division = "(" + sensor.nodeID + ", " + sensor.rssi + ", " + sensor.chipTemperature + "°) " + cell.Division;
                        }
                    }
                }
                else
                {
                    cell.SensorIcon = "";
                    cell.MainValue = "";
                    cell.MainValueSymbol = "";
                    cell.AdditionalValue = "";
                    cell.AdditionalValueSymbol = "";
                }

                cell.SetColor(UIColor.Black);

                if ((item.state.code == MachineStateCodes.Failure) || (item.serviceState.code == MachineServiceStateCodes.Broken))
                    cell.SetColor(UIColor.Red);
                else if ((item.divisionPosition.ID != item.divisionOwner.ID) && (item.divisionPosition.ID != 0))
                    cell.SetColor(UIColor.FromRGBA(1, 0, 0, 0.6f));

                if (item.sensors.Count != 0)
                    if ((DateTime.Now - item.sensors[0].lastTime).TotalMinutes > Settings.greyOfflineMinutes)
                        cell.SetColor(UIColor.Gray);      
                

                cell.UserInteractionEnabled = true;
            }

            cell.SetLayout();

            return cell;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row >= DataManager.machines.Count)
                return;
            
            Application.selectedMachine = DataManager.machines [indexPath.Row];

            if (Application.selectedMachine.type.code == MachineTypeCodes.Cam)
            {
                CamController.selectedCamTime = DateTime.MinValue;
                parentController?.NavigationController?.PushViewController(parentController?.Storyboard?.InstantiateViewController("CamController"), true);
            }
            else
                parentController?.NavigationController?.PushViewController(parentController?.Storyboard?.InstantiateViewController("MachineDetailsController"), true);
        }
    }
}