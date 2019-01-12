using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using DataUtils;
using CoreGraphics;

namespace pbDispatcher.iOS
{
    public partial class PhotoController : UIViewController
    {
        private byte needDataUpdate = 0;
        DateTime lastUpdateTime = DateTime.MinValue;
        string titleStr = "Фотографии оборудования";

        List<MachinePhoto> machinePhotos = new List<MachinePhoto>();

        public PhotoController(IntPtr handle) : base(handle)
        {
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidLoad()
        {
            //MachinePhotosCollection.RegisterClassForCell (typeof(AnimalCell), animalCellId);
            MachinePhotosCollection.Source = new MachinePhotosCollectionSource(machinePhotos, OnRowSelected);

            UpdateViewValues();

            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = titleStr;

            SetLayout();

            DataManager.SheduleGetMachinePhotosListRequest(Application.selectedMachine.ID, DataUpdateCallback);

            UpdateViewValues();

            Application.StartUpdateTimer(CheckNewData);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            SetLayout();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLayout()
        {
            ViewUtils.ExpandWidth(InventoryIDLabel, 3);
            ViewUtils.ExpandWidth(MachineNameLabel, 3);
            ViewUtils.ExpandWidth(DivisionLabel, 3);

            ViewUtils.ExpandWidth(MachinePhotosCollection, 2);
            ViewUtils.ExpandHeight(MachinePhotosCollection, 60);

            AddPhotoButton.Frame = new CGRect(0, MachinePhotosCollection.Frame.Bottom + 5, UIScreen.MainScreen.Bounds.Width, 50);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DataUpdateCallback(object requestResult)
        {
            if (requestResult is List<MachinePhoto>)
            {
                machinePhotos.Clear();
                foreach (var item in (List<MachinePhoto>)requestResult)
                {
                    machinePhotos.Add(item);
                    DataManager.SheduleGetMachinePhotoRequest(Application.selectedMachine.ID, item.photoID, DataUpdateCallback);
                }
                needDataUpdate++;
            }
            else if (requestResult is MachinePhoto)
            {
                if (((MachinePhoto)requestResult).imageData.length != 0)
                {
                    foreach (var item in machinePhotos)
                    {
                        if (item.photoID == ((MachinePhoto)requestResult).photoID)
                        {
                            item.imageData = ((MachinePhoto)requestResult).imageData;
                            break;
                        }
                    }

                    needDataUpdate++;
                }
            }

        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckNewData()
        {
            Title = ViewUtils.FormTitle(titleStr);
            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void UpdateViewValues()
        {
            InventoryIDLabel.Text = "Инв. №: " + Application.selectedMachine.inventoryID;
            if (Application.selectedMachine.sensors.Count != 0)
                InventoryIDLabel.Text += " (" + Application.selectedMachine.sensors[0].nodeID + ")";

            MachineIcon.Image = UIImage.FromFile(Application.selectedMachine.type.iconName);
            MachineNameLabel.Text = Application.selectedMachine.GetNameStr();
            DivisionLabel.Text = Application.selectedMachine.GetDivisionStr();

            MachinePhotosCollection.ReloadData();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void OnRowSelected()
        {
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("PhotoOneController"), true);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        partial void AddPhotoButton_TouchUpInside(UIButton sender)
        {
            NavigationController?.PushViewController(Storyboard?.InstantiateViewController("AddPhotoController"), true);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class MachinePhotosCollectionSource : UICollectionViewSource
    {
        string cellIdentifier = "MachinePhotosCell";

        public List<MachinePhoto> dataSource;
        Action onRowSelected;

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public MachinePhotosCollectionSource(List<MachinePhoto> dataSource, Action onRowSelected)
        {
            this.dataSource = dataSource;
            this.onRowSelected = onRowSelected;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return dataSource.Count;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (MachinePhotosCollectionCell)collectionView.DequeueReusableCell(cellIdentifier, indexPath);

            if (indexPath.Row >= dataSource.Count)
            {
                Console.WriteLine("indexPath.Row >= dataSource.Count " + indexPath.Row + ", " + dataSource.Count);
                return cell;
            }

            var item = dataSource[(ushort)indexPath.Row];

            cell.Time = item.time.ToString("dd.MM.yy  HH:mm:ss");

            if (item.comment == null)
                cell.Author = item.authorName;
            else
                cell.Author = item.comment;

            if ((item.imageData != null) && (item.imageData.length != 0))
                cell.Image = UIImage.LoadFromData(NSData.FromArray(item.imageData.data));
            else
                cell.Image = null;


            return cell;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath.Row >= dataSource.Count)
            {
                Console.WriteLine("indexPath.Row >= dataSource.Count " + indexPath.Row + ", " + dataSource.Count);
                return;
            }

            if ((dataSource[(ushort)indexPath.Row].imageData == null) || (dataSource[(ushort)indexPath.Row].imageData.length == 0))
            {
                Console.WriteLine("imageData == null");
                return;
            }

            Application.selectedMachinePhoto = dataSource[(ushort)indexPath.Row];

            onRowSelected?.Invoke();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }
    }
}