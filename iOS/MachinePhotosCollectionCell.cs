using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace pbDispatcher.iOS
{
    public partial class MachinePhotosCollectionCell : UICollectionViewCell
    {
        public MachinePhotosCollectionCell(IntPtr handle) : base(handle)
        {
        }

        public string Time
        {
            get { return TimeLabel.Text; }
            set { TimeLabel.Text = value; }
        }

        public string Author
        {
            get { return AuthorLabel.Text; }
            set { AuthorLabel.Text = value; }
        }

        public UIImage Image
        {
            set { ImageCell.Image = value; }
        }

        public void SetLayout()
        {
            //ViewUtils.ExpandWidth(UserCell, 3);
        }
    }
}