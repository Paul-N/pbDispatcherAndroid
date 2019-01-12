using Android.App;
using Android.Content;
using System;

namespace Dispatcher.Android.Helpers
{
    public class DialogHelper
    {
        public static void ShowDialog(Context context, string caption, string message, string button, Action callback)
        {
            var dialog = new AlertDialog.Builder(context);  
            var alert = dialog.Create();  
            alert.SetTitle(caption);  
            alert.SetMessage(message);
            alert.SetButton(button, (c, ev) => { callback?.Invoke(); });
            alert.Show();  
        }
    }
}