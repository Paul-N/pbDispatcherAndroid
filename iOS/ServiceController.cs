using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class ServiceController : UIViewController
    {
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;
        MachineServiceStateCodes confirmState;
        MachineServiceStateCodes rejectState;

        public ServiceController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            Title = "Сервисный инцидент";

            ConfirmButton.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            ConfirmButton.TitleLabel.Lines = 2;
            ConfirmButton.TitleLabel.TextAlignment = UITextAlignment.Center;

            RejectButton.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            RejectButton.TitleLabel.Lines = 2;
            RejectButton.TitleLabel.TextAlignment = UITextAlignment.Center;
                         
            CommentField.Layer.BorderColor = UIColor.LightGray.CGColor;
            CommentField.Layer.BorderWidth = 1f;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetLayout();

            UpdateButtonValues();

            Application.StartUpdateTimer(CheckNewData);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
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
            ViewUtils.ExpandWidth(StateCommentLabel, 3);
            ViewUtils.ExpandWidth(StateUserLabel, 3);
            ViewUtils.ExpandWidth(CommentField, 3);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestResult)
        {
            needDataUpdate++;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                Title = "Сервисный инцидент";
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                Title = "Нет авторизации";
            else
                Title = "Нет связи";

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateButtonValues()
        {
            if (Application.selectedMachine.serviceState.code == MachineServiceStateCodes.Work)
            {
                ConfirmButton.SetTitle("Заявить о неисправности", UIControlState.Normal);
                RejectButton.SetTitle("", UIControlState.Normal);
                RejectButton.Enabled = false;

                confirmState = MachineServiceStateCodes.Broken;
                rejectState = MachineServiceStateCodes.None;
            }
            else if (Application.selectedMachine.serviceState.code == MachineServiceStateCodes.Broken)
            {
                ConfirmButton.SetTitle("Принять на ремонт", UIControlState.Normal);
                RejectButton.SetTitle("Отказать в ремонте", UIControlState.Normal);
                RejectButton.Enabled = true;

                confirmState = MachineServiceStateCodes.Service;
                rejectState = MachineServiceStateCodes.Work;
            }
            else if (Application.selectedMachine.serviceState.code == MachineServiceStateCodes.Service)
            {
                ConfirmButton.SetTitle("Вернуть в эксплуатацию", UIControlState.Normal);
                RejectButton.SetTitle("Списать", UIControlState.Normal);
                RejectButton.Enabled = true;

                confirmState = MachineServiceStateCodes.Work;
                rejectState = MachineServiceStateCodes.Offline;
            }
            else if (Application.selectedMachine.serviceState.code == MachineServiceStateCodes.Offline)
            {
                ConfirmButton.SetTitle("Вернуть в эксплуатацию", UIControlState.Normal);
                RejectButton.SetTitle("", UIControlState.Normal);
                RejectButton.Enabled = false;

                confirmState = MachineServiceStateCodes.Work;
                rejectState = MachineServiceStateCodes.None;
            }

        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues()
        {
            InventoryIDLabel.Text = "Инв. №: " + Application.selectedMachine.inventoryID;
            if (Application.selectedMachine.sensors.Count != 0)
                InventoryIDLabel.Text += " (" + BitConverter.ToString(BitConverter.GetBytes(Application.selectedMachine.sensors[0].nodeID), 1, 1)
                    + BitConverter.ToString(BitConverter.GetBytes(Application.selectedMachine.sensors[0].nodeID), 0, 1) + ")";

            var item = Application.selectedMachine;

            StateIcon.Image = UIImage.FromFile(item.serviceState.iconName); 
            StateCommentLabel.Text = item.serviceState.name;

            StateTimeStartLabel.Text = item.serviceStateTimeStart.ToString("dd.MM.yy  HH:mm:ss");
            StateUserLabel.Text = item.userName;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ConfirmButton_TouchUpInside(UIButton sender)
        {
            DataManager.SheduleSetMachineServiceStateRequest(Application.selectedMachine, confirmState, CommentField.Text, null);
            NavigationController?.PopViewController(true);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void RejectButton_TouchUpInside(UIButton sender)
        {
            DataManager.SheduleSetMachineServiceStateRequest(Application.selectedMachine, rejectState, CommentField.Text, null);
            NavigationController?.PopViewController(true);
        }
    }
}