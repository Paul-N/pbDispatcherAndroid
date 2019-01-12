using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Appl;
using Dispatcher.Android.Helpers;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SensorAlertsActivity : BaseActivity
    {
        private const float UpdateInterval = 100f;
        private TimerHolder _timerHolder;
        
        private Machine _machine;
        
        private Sensor _sensor = new Sensor();
        private SensorBordersList _sensorBorders = new SensorBordersList();
        private SensorBorder _sensorBorder = new SensorBorder();

        private TextView _tvMachineNumber;
        private ImageView _ivMachineIcon;
        private TextView  _tvMachineName;
        private TextView  _tvDivisionName;
        private EditText  _etToleranceFromValue;
        private ImageView _ivSensorIcon;
        private EditText  _etToValue;
        private TextView _tvValueSymbolFrom;
        private TextView _tvValueSymbolTo;
        private EditText  _etHour;
        private EditText  _etMin;
        private EditText  _etSec;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _timerHolder = new TimerHolder(UpdateInterval, UpdateConnectionStatus);
            
            _sensorBorders = new SensorBordersList();
            _sensorBorder = new SensorBorder();

            SetContentView(Resource.Layout.activity_sensor_alerts);
            InitActionBar(Resource.String.sensor_alerts);
            
            FindViewById<TextView>(Resource.Id.tvSave)
                .Click += (sender, args) => Save();
            FindViewById<TextView>(Resource.Id.tvCancel)
                .Click += (sender, args) => Cancel();

            _tvMachineNumber = FindViewById<TextView>(Resource.Id.tvMachineNumberAlert);
            _ivMachineIcon = FindViewById<ImageView>(Resource.Id.ivMachineIcon);
            _tvMachineName = FindViewById<TextView>(Resource.Id.tvMachineNameAlert);
            _tvDivisionName = FindViewById<TextView>(Resource.Id.tvDescriptionAlert);
            _etToleranceFromValue = FindViewById<EditText>(Resource.Id.etToleranceFromValue);
            _ivSensorIcon = FindViewById<ImageView>(Resource.Id.ivSensorIcon);
            _etToValue = FindViewById<EditText>(Resource.Id.etToValue);
            _tvValueSymbolFrom = FindViewById<TextView>(Resource.Id.tvValueSymbolFrom);
            _tvValueSymbolTo = FindViewById<TextView>(Resource.Id.tvValueSymbolTo);
            _etHour = FindViewById<EditText>(Resource.Id.etHour);
            _etHour.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(4), new MinMaxInputFilter(0, 9999)});
            _etMin = FindViewById<EditText>(Resource.Id.etMin);
            _etMin.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 59)});
            _etSec = FindViewById<EditText>(Resource.Id.etSec);
            _etSec.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 59)});

            InitCurrentMachine();
            InitDataUpdating();
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
        
        protected override void InitDataUpdating()
        {
            DataManager.SheduleGetSensorBordersRequest(_sensor, DataUpdateCallback);
            UpdateViewValues();
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

            _tvMachineNumber.Text = $"Инв. №: {_machine.inventoryID}{sensorsCount}";

            if (_machine != null && _machine.sensors.Count != 0)
                _sensor = _machine.sensors[0];

            _ivSensorIcon.Visibility = ViewStates.Invisible;            
        }

        private void DataUpdateCallback(object requestResult)
        {
            if (requestResult is SensorBordersList == false) return;

            _sensorBorder = new SensorBorder();

            if (((SensorBordersList)requestResult).list.Count == 0)
            {
                if (_sensor.ID != 0)
                {
                    _sensorBorders = new SensorBordersList
                    {
                        maxSecondsNotOkValue = 1
                    };
                    _sensorBorders.list.Add(new SensorBorder(SensorBorderTypes.Ok, 0, 0));
                    _sensorBorder = _sensorBorders.list[0];
                }
                
                return;
            }

            _sensorBorders = (SensorBordersList)requestResult;

            foreach (var item in _sensorBorders.list)
            {
                if (item.type == SensorBorderTypes.Ok)
                {
                    _sensorBorder = item;
                    break;
                }
            }

            UpdateViewValuesInUiThread();
        }

        private void UpdateConnectionStatus()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                UpdateTitle(Resource.String.sensor_alerts);
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                UpdateTitle(Resource.String.no_authorization);
            else
                UpdateTitle(Resource.String.no_connection);
        }
        
        private void UpdateViewValues()
        {
            _tvMachineNumber.Text = "Инв. №: " + _machine.inventoryID;
            if (_machine.sensors.Count != 0)
                _tvMachineNumber.Text += " (" + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 1, 1)
                                              + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 0, 1) + ")";

            _ivSensorIcon.SetImageBitmap(ResourcesHelper.GetImageFromResources(_sensor.type.iconName));
            _ivMachineIcon.SetImageBitmap(ResourcesHelper.GetImageFromResources(_machine.type.iconName));
            
            _tvMachineName.Text = _machine.GetNameStr();
            _tvDivisionName.Text    = _machine.GetDivisionStr();

            var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
            numberFormatInfo.NumberGroupSeparator = " ";
            numberFormatInfo.NumberDecimalSeparator = ",";

            if (_sensorBorder.minValue == 0 && _sensorBorder.maxValue == 0 && _sensorBorders.maxSecondsNotOkValue == 0) return;

            _etToleranceFromValue.Text = _sensorBorder.minValue.ToString("N", numberFormatInfo);
            _etToValue.Text = _sensorBorder.maxValue.ToString("N", numberFormatInfo);

            _tvValueSymbolFrom.Text = _sensor.type.mainValueSymbol;
            _tvValueSymbolTo.Text   = _sensor.type.mainValueSymbol;

            var seconds = _sensorBorders.maxSecondsNotOkValue;
            var hours = seconds / 3600;
            seconds -= hours * 3600;
            
            var minutes = seconds / 60;
            seconds -= minutes * 60;

            _etHour.Text   = hours.ToString();
            _etMin.Text = minutes.ToString("D2");
            _etSec.Text = seconds.ToString("D2");

            _ivSensorIcon.Visibility = ViewStates.Visible;
        }
        
        private void UpdateViewValuesInUiThread()
        {
            RunOnUiThread(UpdateViewValues);
        }
        
        private async void Save()
        {
            var from = _etToleranceFromValue.Text?.Replace(" ", "").Replace(".", ",");
            var to = _etToValue.Text?.Replace(" ", "").Replace(".", ",");

            if (double.TryParse(from, out var min))
                _sensorBorder.minValue = min;
            if (double.TryParse(to, out var max))
                _sensorBorder.maxValue = max;

            if (int.TryParse(_etHour.Text, out var hours) && 
                int.TryParse(_etMin.Text, out var minutes) &&
                int.TryParse(_etSec.Text, out var seconds))
                _sensorBorders.maxSecondsNotOkValue = hours * 3600 + minutes * 60 + seconds;

            DataManager.SheduleSetSensorBordersRequest(_sensor, _sensorBorders, DataUpdateCallback);
            DataManager.SheduleGetSensorBordersRequest(_sensor, DataUpdateCallback);

            _ivSensorIcon.Visibility = ViewStates.Invisible;
            
            await Task.Delay(500);
            OnBackPressed();
        }
        
        private void Cancel()
        {
            OnBackPressed();
        }
    }
}