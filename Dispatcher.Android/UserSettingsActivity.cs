using System;
using System.Timers;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class UserSettingsActivity : BaseActivity
    {
        private Timer timer;
        private byte needDataUpdate;

        private Button changeUserButton;

        private Button serverButton;

        private TextView userNameText;

        private EditText loginEdit;

        private EditText passwordEdit;

        private EditText warningHourFrom;

        private EditText warningMinuteFrom;

        private EditText warningHourTo;

        private EditText warningMinuteTo;

        private Switch warningSwitch;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_user);
            InitActionBar();
                        
            userNameText = FindViewById<TextView>(Resource.Id.userNameLabel);

            loginEdit = FindViewById<EditText>(Resource.Id.loginEdit);

            passwordEdit = FindViewById<EditText>(Resource.Id.passEdit);

            changeUserButton = FindViewById<Button>(Resource.Id.changeUserButton);

            serverButton = FindViewById<Button>(Resource.Id.serverButton);

            warningHourFrom = FindViewById<EditText>(Resource.Id.warningTime1);
            warningHourFrom.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 23), });

            warningMinuteFrom = FindViewById<EditText>(Resource.Id.warningTime2);
            warningMinuteFrom.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 59) });

            warningHourTo = FindViewById<EditText>(Resource.Id.warningTime3);
            warningHourTo.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 23) });

            warningMinuteTo = FindViewById<EditText>(Resource.Id.warningTime4);
            warningMinuteTo.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 59) });

            warningSwitch = FindViewById<Switch>(Resource.Id.warningSwitch);            

            changeUserButton.Click += (sender, args) =>
            {
                userNameText.SetText(Resource.String.updating);

                DataManager.ChangeUser(loginEdit.Text, passwordEdit.Text, ChangeUserCallback);
            };

            serverButton.Click += (sender, args) =>
            {
                DataManager.SwitchToNextServer();
                DataUtils.StoreValues();
                
                serverButton.SetText(Resource.String.loading);
                
                DataManager.ReconnectToServer(ChangeServerCallback);
            };

            UpdateViewValues();
        }

        protected override void InitDataUpdating() { }

        protected override void OnStart()
        {
            base.OnStart();

            DataManager.SheduleGetUserSettingsRequest(DataUpdateCallback);

            timer = new Timer { AutoReset = true, Interval = 200f };
            timer.Elapsed += delegate { CheckNewData(); };
            timer.Start();

            if (DataManager.LoginImpossible)
            {
                loginEdit.Hint = "demo";
                passwordEdit.Hint = "demo";
            }
            
            UpdateTitle(Resource.String.user_title);
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (DataManager.LoginImpossible == false)
            {
                DataManager.userSettings.notificationsFrom = new DateTime(1970, 1, 1, sbyte.Parse(warningHourFrom.Text), sbyte.Parse(warningMinuteFrom.Text), 0);
                DataManager.userSettings.notificationsTo = new DateTime(1970, 1, 1, sbyte.Parse(warningHourTo.Text), sbyte.Parse(warningMinuteTo.Text), 0);
                DataManager.userSettings.sendNodesOfflineNotifications = warningSwitch.Checked;

                DataManager.SheduleSetUserSettingsRequest(DataManager.userSettings.notificationsFrom, DataManager.userSettings.notificationsTo, DataManager.userSettings.sendNodesOfflineNotifications, DataUpdateCallback);
            }
            else
                DataManager.ChangeUser(loginEdit.Text, passwordEdit.Text, ChangeUserCallback);

            timer.Stop();
            timer.Dispose();
        }

        private void ChangeUserCallback(object requestState)
        {
            DataUtils.StoreValues();

            DataManager.SheduleGetUserSettingsRequest(DataUpdateCallback);
            DataManager.SheduleGetCommonTypesRequest(null);
            DataManager.SheduleGetDivisionsRequest(null);
        }

        private void ChangeServerCallback(object requestState)
        {
            needDataUpdate++;

            DataManager.SheduleGetCommonTypesRequest(null);
            DataManager.SheduleGetDivisionsRequest(null);
        }

        private void DataUpdateCallback(object requestState)
        {
            //if (requestState == RequestStates.Completed)
            needDataUpdate++;
        }

        private void CheckNewData()
        {
            switch (DataManager.ConnectState)
            {
                case ConnectStates.AuthPassed:
                    UpdateTitle(Resource.String.user_title);
                    break;
                case ConnectStates.SocketConnected:
                    UpdateTitle(Resource.String.no_authorization);
                    break;
                default:
                    UpdateTitle(Resource.String.no_connection);
                    break;
            }           

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValuesOnUiThread();
            }
        }
        
        private void UpdateViewValuesOnUiThread()
        {
            RunOnUiThread(UpdateViewValues);
        }

        private void UpdateViewValues()
        {
            serverButton.Text = DataManager.ServerName;

            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                userNameText.Text = DataManager.UserName;
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                userNameText.SetText(Resource.String.no_connection);
            else
                userNameText.SetText(Resource.String.no_connection);

            warningHourFrom.Text = DataManager.userSettings.notificationsFrom.Hour.ToString("D2");
            warningMinuteFrom.Text = DataManager.userSettings.notificationsFrom.Minute.ToString("D2");
            warningHourTo.Text = DataManager.userSettings.notificationsTo.Hour.ToString("D2");
            warningMinuteTo.Text = DataManager.userSettings.notificationsTo.Minute.ToString("D2");
            warningSwitch.Checked = DataManager.userSettings.sendNodesOfflineNotifications;
        }
    }
}