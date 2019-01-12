using Foundation;
using System;
using UIKit;

namespace pbDispatcher.iOS
{
    public partial class MachineStatesLogTableCell : UITableViewCell
    {
        public MachineStatesLogTableCell (IntPtr handle) : base (handle)
        {
        }

        public string Icon
        {
            get { return IconCell.Image.ToString(); }
            set { IconCell.Image = UIImage.FromFile(value); }
        }

        public string Comment
        {
            get { return CommentCell.Text; }
            set { CommentCell.Text = value; }
        }

        public string Time
        {
            get { return TimeCell.Text; }
            set { TimeCell.Text = value; }
        }

        public string Period
        {
            get { return PeriodCell.Text; }
            set 
            { 
                PeriodCell.Text = value;
                if (value != null)
                    PeriodIconCell.Image = UIImage.FromFile("SandWatch.png");
                else
                    PeriodIconCell.Image = null;
            }
        }

        public string User
        {
            get { return UserCell.Text; }
            set { UserCell.Text = value; }
        }

        public void SetLayout()
        {
            ViewUtils.ExpandWidth(CommentCell, 3);
            ViewUtils.ExpandWidth(UserCell, 3);
        }
    }
}