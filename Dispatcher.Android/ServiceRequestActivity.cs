using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Appl;
using Dispatcher.Android.Helpers;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
    public class ServiceRequestActivity : BaseActivity
    {
        private const float UpdateInterval = 100f;
        private TimerHolder _timerHolder;
        private byte _needDataUpdate;
        
        private Machine _machine;
        
        private MachineServiceStateCodes _confirmState;
        private MachineServiceStateCodes _rejectState;
        
        private TextView _tvTakeForRepair;
        private TextView _tvRefuseToRepair;
        private TextView _tvMachineNumberRequest;
        private ImageView _ivStateRequest;
        private TextView _tvStateNameRequest;
        private TextView _tvStartTimeRequest;
        private TextView _tvUserRequest;
        private EditText _etRequest;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _timerHolder = new TimerHolder(UpdateInterval, CheckNewData);

            SetContentView(Resource.Layout.activity_service_request);
            InitActionBar(Resource.String.service_intend);
            
            _tvTakeForRepair = FindViewById<TextView>(Resource.Id.tvTakeForRepair);
            _tvRefuseToRepair = FindViewById<TextView>(Resource.Id.tvRefuseToRepair);

            _tvTakeForRepair.Click += (sender, args) => TakeForRepair();
            _tvRefuseToRepair.Click += (sender, args) => RefuseToRepair();
            
            _tvMachineNumberRequest = FindViewById<TextView>(Resource.Id.tvMachineNumberRequest);
            _ivStateRequest = FindViewById<ImageView>(Resource.Id.ivStateRequest);
            _tvStateNameRequest = FindViewById<TextView>(Resource.Id.tvStateNameRequest);
            _tvStartTimeRequest = FindViewById<TextView>(Resource.Id.tvStartTimeRequest);
            _tvUserRequest = FindViewById<TextView>(Resource.Id.tvUserRequest);
            _etRequest = FindViewById<EditText>(Resource.Id.etRequest);
            
            InitCurrentMachine();
            InitDataUpdating();
        }
        
        protected override void InitDataUpdating()
        {
            DataManager.SheduleGetMachineStatesLogRequest(
                _machine, 
                Settings.machineStatesLogMaxElements, 
                DataUpdateCallback);
            
            UpdateButtonValues();
            UpdateViewValues();
            
            _timerHolder.Start();
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            _timerHolder.Start();            
        }
        
        protected override void OnStop()
        {
            base.OnStop();

            _timerHolder.Stop();
        }
        
        private void InitCurrentMachine()
        {
            _machine = AppSession.SelectedMachine;

            string sensorsCount = null;
            if (_machine.sensors.Count != 0)
            {
                sensorsCount = " (" + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 1, 1)
                                    + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 0, 1) + ")";
            }               

            FindViewById<TextView>(Resource.Id.tvMachineNumberRequest)
                .Text = $"Инв. №: {_machine.inventoryID}{sensorsCount}";
        }
        
        private void DataUpdateCallback(object requestResult)
        {
            if (!(requestResult is List<MachineStatesLogElement> stateList)) return;

            var state = stateList
                .FirstOrDefault(st => st.timeEnd == DateTime.MinValue && 
                                      st.state is MachineServiceState);
            
            if (state == null) return;

            _machine.serviceState = (MachineServiceState)state.state;
            _machine.serviceStateTimeStart = state.timeStart;
            _machine.userName = state.userName;
                
            _needDataUpdate++;
        }
        
        private void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                UpdateTitle(Resource.String.service_intend);
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                UpdateTitle(Resource.String.no_authorization);
            else
                UpdateTitle(Resource.String.no_connection);

            if (_needDataUpdate > 0)
            {
                _needDataUpdate--;
                UpdateViewValuesInUiThread();
            }
        }
        
        private void UpdateButtonValues()
        {
            if (_machine.serviceState.code == MachineServiceStateCodes.Work)
            {
                _tvTakeForRepair.Text = "Заявить о неисправности";
                _tvRefuseToRepair.Text = string.Empty;
                _tvRefuseToRepair.Enabled = false;

                _confirmState = MachineServiceStateCodes.Broken;
                _rejectState = MachineServiceStateCodes.None;
            }
            else if (_machine.serviceState.code == MachineServiceStateCodes.Broken)
            {
                _tvTakeForRepair.Text = "Принять на ремонт";
                _tvRefuseToRepair.Text = "Отказать в ремонте";
                _tvRefuseToRepair.Enabled = true;

                _confirmState = MachineServiceStateCodes.Service;
                _rejectState = MachineServiceStateCodes.Work;
            }
            else if (_machine.serviceState.code == MachineServiceStateCodes.Service)
            {
                _tvTakeForRepair.Text = "Вернуть в эксплуатацию";
                _tvRefuseToRepair.Text = "Списать";
                _tvRefuseToRepair.Enabled = true;

                _confirmState = MachineServiceStateCodes.Work;
                _rejectState = MachineServiceStateCodes.Offline;
            }
            else if (_machine.serviceState.code == MachineServiceStateCodes.Offline)
            {
                _tvTakeForRepair.Text = "Вернуть в эксплуатацию";
                _tvRefuseToRepair.Text = string.Empty;
                _tvRefuseToRepair.Enabled = false;

                _confirmState = MachineServiceStateCodes.Work;
                _rejectState = MachineServiceStateCodes.None;
            }
        }
        
        private void UpdateViewValues()
        {
            _tvMachineNumberRequest.Text = "Инв. №: " + _machine.inventoryID;
            if (_machine.sensors.Count != 0)
                _tvMachineNumberRequest.Text += " (" + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 1, 1)
                                              + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 0, 1) + ")";
            
            _ivStateRequest.SetImageBitmap(ResourcesHelper.GetImageFromResources(_machine.serviceState.iconName));
            _tvStateNameRequest.Text = _machine.serviceState.name;
            _tvStartTimeRequest.Text = _machine.serviceStateTimeStart.ToString("dd.MM.yy  HH:mm:ss");
            _tvUserRequest.Text = _machine.userName;
        }

        private void UpdateViewValuesInUiThread()
        {
            RunOnUiThread(UpdateViewValues);
        }

        private void TakeForRepair()
        {
            DataManager.SheduleSetMachineServiceStateRequest(_machine, _confirmState, _etRequest.Text, null);
            OnBackPressed();
        }
        
        private void RefuseToRepair()
        {
            DataManager.SheduleSetMachineServiceStateRequest(_machine, _rejectState, _etRequest.Text, null);
            OnBackPressed();
        }
    }
}