using Android.Support.V7.App;
using Android.Widget;

namespace Dispatcher.Android
{
    public abstract class BaseActivity : AppCompatActivity
    {
        private ImageView _ivBack;
        private TextView _tvBack;
        private TextView _tvTitle;

        protected void InitActionBar()
        {
            if (_ivBack == null)
            {
                _ivBack = FindViewById<ImageView>(Resource.Id.ivBack);
                _ivBack.Click += (sender, args) => OnBackPressed();
            }

            if (_tvBack == null)
            {
                _tvBack = FindViewById<TextView>(Resource.Id.tvBack);
                _tvBack.Click += (sender, args) => OnBackPressed();
            }

            if (_tvTitle == null)
                _tvTitle = FindViewById<TextView>(Resource.Id.tvActionBarTitle);
        }
        
        protected void InitActionBar(int title)
        {
            InitActionBar();
            _tvTitle.SetText(title);
        }

        protected void InitActionBar(string title)
        {
            InitActionBar();
            _tvTitle.Text = title;
        }
        
        protected void UpdateTitle(int resourceId)
        {
            RunOnUiThread(() => _tvTitle.SetText(resourceId));
        }

        protected void UpdateTitle(string title)
        {
            RunOnUiThread(() => _tvTitle.Text = title);
        }

        protected abstract void InitDataUpdating();
    }
}