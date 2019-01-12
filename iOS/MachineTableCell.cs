using Foundation;
using System;
using UIKit;

namespace pbDispatcher.iOS
{
    public partial class MachineTableCell : UITableViewCell
    {
        public MachineTableCell(IntPtr handle) : base(handle)
        {
        }

        public string MachineIcon
        {
            get { return MachineIconCell.Image.ToString(); }
            set { MachineIconCell.Image = UIImage.FromFile(value); }
        }

        public string MachineStateIcon
        {
            get { return MachineStateIconCell.Image.ToString(); }
            set { MachineStateIconCell.Image = UIImage.FromFile(value); }
        }

        public string SensorIcon
        {
            get { return SensorIconCell.Image.ToString(); }
            set { SensorIconCell.Image = UIImage.FromFile(value); }
        }

        public string Name
        {
            get { return NameCell.Text; }
            set { NameCell.Text = value; }
        }

        public string Division
        {
            get { return DivisionCell.Text; }
            set { DivisionCell.Text = value; }
        }

        public string MainValue
        {
            get { return MainValueCell.Text; }
            set { MainValueCell.Text = value; }
        }

        public string MainValueSymbol
        {
            get { return MainValueSymbolCell.Text; }
            set { MainValueSymbolCell.Text = value; }
        }

        public string AdditionalValue
        {
            get { return AdditionalValueCell.Text; }
            set { AdditionalValueCell.Text = value; }
        }

        public string AdditionalValueSymbol
        {
            get { return AdditionalValueSymbolCell.Text; }
            set { AdditionalValueSymbolCell.Text = value; }
        }



        public void SetColor(UIColor color)
        {
            NameCell.TextColor = color;
            DivisionCell.TextColor = color;
            MainValueCell.TextColor = color;
            MainValueSymbolCell.TextColor = color;
            AdditionalValueCell.TextColor = color;
            AdditionalValueSymbolCell.TextColor = color;
        }


        public void SetLayout()
        {
            SensorIconCell.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - SensorIconCell.Frame.Width - MainValueCell.Frame.Width - MainValueSymbolCell.Frame.Width + 10, SensorIconCell.Frame.Y, SensorIconCell.Frame.Width, SensorIconCell.Frame.Height);
            MainValueCell.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - MainValueCell.Frame.Width - MainValueSymbolCell.Frame.Width - 2, MainValueCell.Frame.Y, MainValueCell.Frame.Width, MainValueCell.Frame.Height);
            MainValueSymbolCell.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - MainValueSymbolCell.Frame.Width, MainValueSymbolCell.Frame.Y, MainValueSymbolCell.Frame.Width, MainValueSymbolCell.Frame.Height);

            AdditionalValueCell.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - AdditionalValueCell.Frame.Width - AdditionalValueSymbolCell.Frame.Width - 2, AdditionalValueCell.Frame.Y, AdditionalValueCell.Frame.Width, AdditionalValueCell.Frame.Height);
            AdditionalValueSymbolCell.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - AdditionalValueSymbolCell.Frame.Width, AdditionalValueSymbolCell.Frame.Y, AdditionalValueSymbolCell.Frame.Width, AdditionalValueSymbolCell.Frame.Height);
        }
    }
}