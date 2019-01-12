using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class UserSettingsController : UIViewController
    {
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;

        public UserSettingsController (IntPtr handle) : base (handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            Title = "Пользователь";
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetLayout();

            UpdateViewValues();

            DataManager.SheduleGetUserSettingsRequest(DataUpdateCallback);

            Application.StartUpdateTimer(CheckNewData);

            if (DataManager.LoginImpossible == true)
            {
                LoginField.Placeholder = "demo";
                PasswordField.Placeholder = "demo";
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (DataManager.LoginImpossible == false)
            {
                try
                {
                    DataManager.userSettings.notificationsFrom = new DateTime(1970, 1, 1, sbyte.Parse(NotificationsFromHoursField.Text), sbyte.Parse(NotificationsFromMinutesField.Text), 0);
                    DataManager.userSettings.notificationsTo = new DateTime(1970, 1, 1, sbyte.Parse(NotificationsToHoursField.Text), sbyte.Parse(NotificationsToMinutesField.Text), 0);
                    DataManager.userSettings.sendNodesOfflineNotifications = SendNodesOfflineNotificationsSwitch.On;

                    DataManager.SheduleSetUserSettingsRequest(DataManager.userSettings.notificationsFrom, DataManager.userSettings.notificationsTo, DataManager.userSettings.sendNodesOfflineNotifications, DataUpdateCallback);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }


            }
            else
                DataManager.ChangeUser(LoginField.Text, PasswordField.Text, ChangeUserCallback);
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
            ViewUtils.ExpandWidth(LoginField, 3);
            ViewUtils.ExpandWidth(PasswordField, 3);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestState)
        {
            //if (requestState == RequestStates.Completed)
                needDataUpdate++;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ChangeUserCallback(object requestState)
        {
            Application.StoreValues();

            DataManager.SheduleGetUserSettingsRequest(DataUpdateCallback);
            DataManager.SheduleGetCommonTypesRequest(null);
            DataManager.SheduleGetDivisionsRequest(null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ChangeServerCallback(object requestState)
        {
            needDataUpdate++;

            DataManager.SheduleGetCommonTypesRequest(null);
            DataManager.SheduleGetDivisionsRequest(null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                Title = "Пользователь";
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
        private void UpdateViewValues()
        {
            ServerButton.SetTitle(DataManager.ServerName, UIControlState.Normal);

            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                UserNameLabel.Text = DataManager.UserName;
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                UserNameLabel.Text = "Нет авторизации";
            else
                UserNameLabel.Text = "Нет связи";
                
            NotificationsFromHoursField.Text = DataManager.userSettings.notificationsFrom.Hour.ToString("D2");
            NotificationsFromMinutesField.Text = DataManager.userSettings.notificationsFrom.Minute.ToString("D2");
            NotificationsToHoursField.Text = DataManager.userSettings.notificationsTo.Hour.ToString("D2");
            NotificationsToMinutesField.Text = DataManager.userSettings.notificationsTo.Minute.ToString("D2");
            SendNodesOfflineNotificationsSwitch.SetState(DataManager.userSettings.sendNodesOfflineNotifications, false);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ChangeUserButtonTouch(UIButton sender)
        {
            UserNameLabel.Text = "Обновление...";

            DataManager.ChangeUser(LoginField.Text, PasswordField.Text, ChangeUserCallback);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void ServerButton_TouchUpInside(UIButton sender)
        {
            DataManager.SwitchToNextServer();
            Application.StoreValues();

            ServerButton.SetTitle(DataManager.ServerName, UIControlState.Normal);
            //ServerButton.SetTitle("Загрузка...", UIControlState.Normal);

            DataManager.ReconnectToServer(ChangeServerCallback);
        }
    }
}