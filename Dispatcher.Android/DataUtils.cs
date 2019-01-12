using System.Threading;
using Android.App;
using Android.Content;
using Android.Preferences;
using DataUtils;

namespace Dispatcher.Android
{
    public class DataUtils
    {
        public static bool IsInit = false;

        static Thread clientThread;
        public static Machine selectedMachine;

        public static void Init()
        {
            if (IsInit) return;

            IsInit = true;

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

            DataManager.storedCityID = (ushort)prefs.GetInt("SelectedCity", 0);
            DataManager.storedDivisionID = (ushort)prefs.GetInt("SelectedDivision", 0);
            DataManager.storedMachineTypeID = (ushort)prefs.GetInt("SelectedMachineType", 0);
            DataManager.storedMachineStateID = (ushort)prefs.GetInt("SelectedMachineState", 0);
            DataManager.storedMachineServiceStateID = (ushort)prefs.GetInt("SelectedMachineServiceState", 0);
            DataManager.onlyMyMachines = prefs.GetBoolean("onlyMyMachines", false);

            Client.userToken = prefs.GetString("UserToken", null);
            Client.currentServerNum = prefs.GetInt("CurrentServer", 0);

            clientThread = new Thread(DataManager.Init);
            clientThread.IsBackground = true;
            clientThread.Start();
        }

        public static void StoreValues()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var editor = prefs.Edit();

            if (DataManager.selectedCity != null)
                editor.PutInt("SelectedCity", (int)DataManager.selectedCity.ID);
            if (DataManager.selectedDivision != null)
                editor.PutInt("SelectedDivision", (int)DataManager.selectedDivision.ID);
            if (DataManager.selectedMachineType != null)
                editor.PutInt("SelectedMachineType", (int)DataManager.selectedMachineType.ID);
            if (DataManager.selectedMachineState != null)
                editor.PutInt("SelectedMachineState", (int)DataManager.selectedMachineState.ID);
            if (DataManager.selectedMachineServiceState != null)
                editor.PutInt("SelectedMachineServiceState", (int)DataManager.selectedMachineServiceState.ID);

            editor.PutBoolean("onlyMyMachines", DataManager.onlyMyMachines);
            editor.PutString("UserToken", Client.userToken);
            editor.PutInt("CurrentServer", DataManager.CurrentServerNum);

            editor.Apply();
        }
    }
}