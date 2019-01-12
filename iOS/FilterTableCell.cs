using Foundation;
using System;
using UIKit;

namespace pbDispatcher.iOS
{
    public partial class FilterTableCell : UITableViewCell
    {
        public FilterTableCell (IntPtr handle) : base (handle)
        {
        }

        public string LeftTitle
        {
            get { return CityCell.Text; }
            set { CityCell.Text = value; }
        }

        public string MainTitle
        {
            get { return NameCell.Text; }
            set { NameCell.Text = value; }
        }

        public string Icon
        {
            get { return IconCell.Image.ToString(); }
            set { IconCell.Image = UIImage.FromFile(value); }
        }

        public void SetMainTitleOnMiddle()
        {
            NameCell.TextAlignment = UITextAlignment.Center;
            NameCell.Frame = new CoreGraphics.CGRect(5, NameCell.Frame.Y, UIScreen.MainScreen.Bounds.Width - 5, NameCell.Frame.Height);
        }

        public void SetLayout()
        {
            NameCell.TextAlignment = UITextAlignment.Left;
            NameCell.Frame = new CoreGraphics.CGRect(41, NameCell.Frame.Y, UIScreen.MainScreen.Bounds.Width - 5, NameCell.Frame.Height);

            ViewUtils.ExpandWidth(NameCell, 3);
        }
    }
}