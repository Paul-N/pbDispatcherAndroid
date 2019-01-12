using Foundation;
using System;
using UIKit;

namespace pbDispatcher.iOS
{
    public partial class UITextFieldWithKey : UITextField
    {
        public string rowKey;

        public UITextFieldWithKey (IntPtr handle) : base (handle)
        {
        }
    }
}