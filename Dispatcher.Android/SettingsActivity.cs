using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Appl;
using Dispatcher.Android.Helpers;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : BaseActivity
    {
        private const float UpdateInterval = 100f;
        private TimerHolder _timerHolder;

        private TextView _tvCity;
        private TextView _tvDivision;
        private TextView _tvFacilityType;
        private TextView _tvWorkState;
        private TextView _tvServiceState;
        private Switch _swOnlyMy;

        private ImageView _ivFacilityType;
        private ImageView _ivWorkSate;
        private ImageView _ivServiceState;
        
        private TextView _tvUserNameValue;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _timerHolder = new TimerHolder(UpdateInterval, CheckNewData);

            SetContentView(Resource.Layout.activity_settings);
            InitActionBar(Resource.String.settings);

            _tvCity = FindViewById<TextView>(Resource.Id.tvCityValue);
            _tvCity.Click += (sender, args) => StartFilterActivity(eFilterType.City);

            _tvDivision = FindViewById<TextView>(Resource.Id.tvUnitValue);
            _tvDivision.Click += (sender, args) => StartFilterActivity(eFilterType.Division);
            _tvFacilityType = FindViewById<TextView>(Resource.Id.tvFacilityTypeValue);
            _tvFacilityType.Click += (sender, args) => StartFilterActivity(eFilterType.FacilityType);
            _tvWorkState = FindViewById<TextView>(Resource.Id.tvWorkSateValue);
            _tvWorkState.Click += (sender, args) => StartFilterActivity(eFilterType.WorkState);
            _tvServiceState = FindViewById<TextView>(Resource.Id.tvServiceStateValue);
            _tvServiceState.Click += (sender, args) => StartFilterActivity(eFilterType.ServiceState);
            _swOnlyMy = FindViewById<Switch>(Resource.Id.swOnlyMy);
            _swOnlyMy.CheckedChange += SwOnlyMyOnCheckedChange; 

            _ivFacilityType = FindViewById<ImageView>(Resource.Id.ivFacilityType);
            _ivWorkSate = FindViewById<ImageView>(Resource.Id.ivWorkSate);
            _ivServiceState = FindViewById<ImageView>(Resource.Id.ivServiceState);

            _tvUserNameValue = FindViewById<TextView>(Resource.Id.tvUserNameValue);
            _tvUserNameValue.Text = DataManager.UserName;
            _tvUserNameValue.Click += (sender, args) => StartUserActivity();

            InitDataUpdating();
        }

        protected override void InitDataUpdating()
        {
            UpdateViewValues();
            UpdateUserValue();
            
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

        private void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                UpdateTitle(Resource.String.settings);
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                UpdateTitle(Resource.String.no_authorization);
            else
                UpdateTitle(Resource.String.no_connection);

            UpdateViewValuesInUiThread();
            UpdateUserValueInUiThread();
        }

        private void UpdateUserValue()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                _tvUserNameValue.Text = DataManager.UserName;
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                _tvUserNameValue.SetText(Resource.String.no_authorization);
            else
                _tvUserNameValue.SetText(Resource.String.no_connection);
        }
        
        private void UpdateViewValues()
        {
            if (DataManager.selectedCity != null)
                _tvCity.Text = DataManager.selectedCity.name;
            if (DataManager.selectedDivision != null)
                _tvDivision.Text = DataManager.selectedDivision.name;
            if (DataManager.selectedMachineType != null)
            {
                _tvFacilityType.Text = DataManager.selectedMachineType.name;
                _ivFacilityType.SetImageBitmap(ResourcesHelper.GetImageFromResources(DataManager.selectedMachineType.iconName));
            }
            if (DataManager.selectedMachineState != null)
            {
                _tvWorkState.Text = DataManager.selectedMachineState.name;
                _ivWorkSate.SetImageBitmap(ResourcesHelper.GetImageFromResources(DataManager.selectedMachineState.iconName));
            }
            if (DataManager.selectedMachineServiceState != null)
            {
                _tvServiceState.Text = DataManager.selectedMachineServiceState.name;
                _ivServiceState.SetImageBitmap(ResourcesHelper.GetImageFromResources(DataManager.selectedMachineServiceState.iconName));
            }

            _swOnlyMy.Checked = DataManager.onlyMyMachines;
        }

        private void UpdateViewValuesInUiThread()
        {
            RunOnUiThread(UpdateViewValues);
        }

        private void UpdateUserValueInUiThread()
        {
            RunOnUiThread(UpdateUserValue);
        }

        private void StartFilterActivity(eFilterType filterType)
        {
            AppSession.SelectedFilterType = filterType;
            
            var intent = new Intent(this, typeof(FilterListActivity));
            StartActivity(intent);
        }

        private void StartUserActivity()
        {
            var intent = new Intent();
            intent.SetClass(BaseContext, typeof(UserSettingsActivity));
            intent.SetFlags(ActivityFlags.ReorderToFront);
            StartActivity(intent);
        }
        
        private void SwOnlyMyOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            DataManager.onlyMyMachines = _swOnlyMy.Checked;
            DataUtils.StoreValues();
        }
    }
}