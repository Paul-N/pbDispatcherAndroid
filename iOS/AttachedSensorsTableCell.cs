using Foundation;
using System;
using UIKit;

namespace pbDispatcher.iOS
{
    public partial class AttachedSensorsTableCell : UITableViewCell
    {
        public AttachedSensorsTableCell (IntPtr handle) : base (handle)
        {
        }

        public string Icon
        {
            get { return IconCell.Image.ToString(); }
            set { IconCell.Image = UIImage.FromFile(value); }
        }

        public string Name
        {
            get { return NameCell.Text; }
            set { NameCell.Text = value; }
        }

        public string Comment
        {
            get { return CommentCell.Text; }
            set { CommentCell.Text = value; }
        }

        public string MainValue
        {
            get { return MainValueCell.Text; }
            set { MainValueCell.Text = value; }
        }

        public string AdditionalValue
        {
            get { return AdditionalValueCell.Text; }
            set { AdditionalValueCell.Text = value; }
        }

        public void SetLayout()
        {
            MainValueCell.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - MainValueCell.Frame.Width - 8, MainValueCell.Frame.Y, MainValueCell.Frame.Width, MainValueCell.Frame.Height);
            AdditionalValueCell.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - AdditionalValueCell.Frame.Width - 8, AdditionalValueCell.Frame.Y, AdditionalValueCell.Frame.Width, AdditionalValueCell.Frame.Height);
        }
    }
}