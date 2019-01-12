using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using DataUtils;

namespace pbDispatcher.iOS
{
    public partial class HistoryGraphView : UIView
    {
		const float leftBorder = 30;
		const float rightBorder = 10;
		const float downBorder = 40;

        private List<HistoryPoint> historyList;
        public DateTime timeStart, timeEnd;
        public bool readyToDataUpdate = false;

        UIPinchGestureRecognizer pinchGesture;
        UIPanGestureRecognizer   panGesture;
        UITapGestureRecognizer   tapGesture;

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public HistoryGraphView(IntPtr handle) : base(handle)
        {
            pinchGesture = new UIPinchGestureRecognizer(HandlePinchAction);
            panGesture = new UIPanGestureRecognizer(HandlePanAction);
            tapGesture = new UITapGestureRecognizer(HandleTapAction);

            this.AddGestureRecognizer(pinchGesture);
            this.AddGestureRecognizer(panGesture);
            this.AddGestureRecognizer(tapGesture);
        }

        DateTime initialPinchTimeStart, initialPinchTimeEnd;
        double relativePos;
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        void HandlePinchAction()
        {
            if (pinchGesture.State == UIGestureRecognizerState.Began)
            {
                initialPinchTimeStart = timeStart;
                initialPinchTimeEnd   = timeEnd;
                relativePos = pinchGesture.LocationInView(this).X / Frame.Size.Width;
            }
            else if (pinchGesture.State == UIGestureRecognizerState.Ended)
            {
                readyToDataUpdate = true;
            }

            double oldFrame = (initialPinchTimeEnd - initialPinchTimeStart).TotalSeconds;
            double newFrame = oldFrame / pinchGesture.Scale;

            if (newFrame > new TimeSpan(2000, 0, 0, 0, 0).TotalSeconds)
                newFrame = oldFrame;

            timeStart = initialPinchTimeStart.AddSeconds((oldFrame - newFrame) * relativePos);

            if (timeStart < DataManager.unixEpoch)
                timeStart = DataManager.unixEpoch;
                
            timeEnd = timeStart.AddSeconds(newFrame);

            SetNeedsDisplay();

            //Console.WriteLine("state: " + pinchGesture.State + ", timeStart: " + timeStart + "timeEnd: " + timeEnd + ", scale: " + pinchGesture.Scale + ", relativePos: " + relativePos);
        }

        DateTime initialPanTimeStart;
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        void HandlePanAction()
        {
            if (panGesture.State == UIGestureRecognizerState.Began)
                initialPanTimeStart = timeStart;
            else if (panGesture.State == UIGestureRecognizerState.Ended)
            {
                readyToDataUpdate = true;
            }
            
            nfloat diffX = panGesture.TranslationInView(this).X;
            double relativeX = diffX / Frame.Size.Width;

            double frame = (timeEnd - timeStart).TotalSeconds;

            timeStart = initialPanTimeStart.AddSeconds(-frame * relativeX);

            if (timeStart < DataManager.unixEpoch)
                timeStart = DataManager.unixEpoch;
                
            timeEnd = timeStart.AddSeconds(frame);

            SetNeedsDisplay();

            //Console.WriteLine("pan state: " + panGesture.State + ", timeStart: " + timeStart);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        void HandleTapAction()
        {
            //Console.WriteLine("tap: " + tapGesture.LocationInView(this).ToString());
        }


      

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetData (List<HistoryPoint> historyList, DateTime timeStart, DateTime timeEnd)
        {
            this.historyList = historyList;
            this.timeStart   = timeStart;
            this.timeEnd     = timeEnd;
        }

        double initialMinValue = double.MaxValue;
        double initialMaxValue = double.MinValue;

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		public override void Draw (CGRect rect)
		{
            DateTime t1 = DateTime.Now;
			base.Draw (rect);

			var context = UIGraphics.GetCurrentContext ();
			var path = new CGPath ();
			CGPoint [] pointsBuff;
			CGPoint [] points;

			float width = (float)Frame.Size.Width - leftBorder - rightBorder;
			float height = (float)Frame.Size.Height - downBorder;

			//X-Y axis
			points = new CGPoint [9];
			points [0] = new CGPoint (leftBorder - 3, 3);
			points [1] = new CGPoint (leftBorder, 0);
			points [2] = new CGPoint (leftBorder + 3, 3);
			points [3] = new CGPoint (leftBorder, 0);
			points [4] = new CGPoint (leftBorder, height + 2);
			points [5] = new CGPoint (leftBorder + width + rightBorder / 2, height + 2);
			points [6] = new CGPoint (leftBorder + width + rightBorder / 2 - 3, height + 2 - 3);
			points [7] = new CGPoint (leftBorder + width + rightBorder / 2, height + 2);
			points [8] = new CGPoint (leftBorder + width + rightBorder / 2 - 3, height + 2 + 3);

			path.AddLines (points);

			context.AddPath (path);

			context.SetLineWidth (1); ;
			context.SetStrokeColor (new CGColor (0, 0, 0, 1));

			context.DrawPath (CGPathDrawingMode.Stroke);

			NSStringDrawing.DrawString (new NSString (timeStart.ToString ("HH:mm")), new CGPoint (leftBorder, height + 5), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (12) });
            NSStringDrawing.DrawString (new NSString (timeStart.ToString ("dd.MM.yy")), new CGPoint (leftBorder, height + 17), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (7) });
            NSStringDrawing.DrawString (new NSString (timeEnd.ToString ("HH:mm")), new CGPoint (leftBorder + width - 25, height + 5), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (12) });
            NSStringDrawing.DrawString (new NSString (timeEnd.ToString ("dd.MM.yy")), new CGPoint (leftBorder + width - 25, height + 17), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (7) });
			
            if ((historyList == null) || (historyList.Count == 0))
                return;
            
            context.SetStrokeColor (UIColor.LightGray.CGColor);
			context.SetLineWidth (2);

            float period = (float) (timeEnd - timeStart).Ticks;

            double minValue = initialMinValue;
            double maxValue = initialMaxValue;
            for (int s = 0; s < historyList.Count; s++)
			{
                if (historyList [s].value < minValue)
                    minValue = historyList [s].value;
                if (historyList [s].value > maxValue)
                    maxValue = historyList [s].value;
			}

            initialMinValue = minValue;
            initialMaxValue = maxValue;
            
            double valueRange = maxValue - minValue;


			path = new CGPath ();
			pointsBuff = new CGPoint [(int) width];

			//double lastX = 0;
			int pointX = 0;

            for (int s = 0; s < historyList.Count; s++)
			{
                if ((historyList[s].time < timeStart) || (historyList[s].time > timeEnd))
                    continue;
                    
                double X = leftBorder + (double) (historyList [s].time - timeStart).Ticks / period * width;
                double Y = height + 2 - ((historyList [s].value - minValue) / valueRange * height);

				/*if ((Math.Abs (lastX - X) < 1))
					continue;

				lastX = X;*/

				if (pointX == (int)width)
					break;
				
				pointsBuff [pointX] = new CGPoint(X, Y);
				pointX++;

				//Console.WriteLine (points [s].ToString ());
			}
			points = new CGPoint[pointX];
			Array.Copy(pointsBuff, points, pointX);

			path.AddLines (points);

			context.AddPath (path);

			context.DrawPath (CGPathDrawingMode.Stroke);



			//X axis agendaa
            DateTime quartDate = timeStart + new TimeSpan (((timeEnd - timeStart).Ticks / 4));
            DateTime mediumDate = timeStart + new TimeSpan (((timeEnd - timeStart).Ticks / 2));
            DateTime threeQuartDate = timeStart + new TimeSpan (((timeEnd - timeStart).Ticks * 3 / 4));

			NSStringDrawing.DrawString (new NSString (quartDate.ToString ("HH:mm")), new CGPoint (leftBorder + width / 4 - 15, height + 5), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (12) });
			NSStringDrawing.DrawString (new NSString (quartDate.ToString ("dd.MM.yy")), new CGPoint (leftBorder + width / 4 - 15, height + 17), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (7) });
			NSStringDrawing.DrawString (new NSString (mediumDate.ToString ("HH:mm")), new CGPoint (leftBorder + width / 2 - 15, height + 5), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (12) });
			NSStringDrawing.DrawString (new NSString (mediumDate.ToString ("dd.MM.yy")), new CGPoint (leftBorder + width / 2 - 15, height + 17), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (7) });
			NSStringDrawing.DrawString (new NSString (threeQuartDate.ToString ("HH:mm")), new CGPoint (leftBorder + width * 3 / 4 - 15, height + 5), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (12) });
			NSStringDrawing.DrawString (new NSString (threeQuartDate.ToString ("dd.MM.yy")), new CGPoint (leftBorder + width * 3 / 4 - 15, height + 17), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (7) });

			path = new CGPath ();

			points = new CGPoint [2];
			points [0] = new CGPoint (leftBorder + width / 4, height + 2 - 3);
			points [1] = new CGPoint (leftBorder + width / 4, height + 2 + 3);
			path.AddLines (points);

			points [0] = new CGPoint (leftBorder + width / 2, height + 2 - 3);
			points [1] = new CGPoint (leftBorder + width / 2, height + 2 + 3);
			path.AddLines (points);

			points [0] = new CGPoint (leftBorder + width * 3 / 4, height + 2 - 3);
			points [1] = new CGPoint (leftBorder + width * 3 / 4, height + 2 + 3);
			path.AddLines (points);

			context.AddPath (path);

			context.SetLineWidth (1); ;
			context.SetStrokeColor (new CGColor (0, 0, 0, 1));

			context.DrawPath (CGPathDrawingMode.Stroke);

			//Y axis agenda
			double quartValue = minValue + ((maxValue - minValue) / 4);
			double mediumValue = minValue + ((maxValue - minValue) / 2);
			double threeQuartValue = minValue + ((maxValue - minValue) * 3 / 4);


			NSMutableParagraphStyle paragraphStyle = new NSMutableParagraphStyle ();

			paragraphStyle.Alignment = UITextAlignment.Right;

			NSStringDrawing.DrawString (new NSString (minValue.ToString ("F1")), new CGRect (2, height - 12, leftBorder - 6, 12), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (11), ParagraphStyle = paragraphStyle });
			NSStringDrawing.DrawString (new NSString (quartValue.ToString ("F1")), new CGRect (2, height - 6 - height / 4, leftBorder - 6, 12), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (11), ParagraphStyle = paragraphStyle});
			NSStringDrawing.DrawString (new NSString (mediumValue.ToString ("F1")), new CGRect (2, height - 6 - height / 2, leftBorder - 6, 12), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (11), ParagraphStyle = paragraphStyle });
			NSStringDrawing.DrawString (new NSString (threeQuartValue.ToString ("F1")), new CGRect (2, height - 6 - height * 3 / 4, leftBorder - 6, 12), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (11), ParagraphStyle = paragraphStyle });
			NSStringDrawing.DrawString (new NSString (maxValue.ToString ("F1")), new CGRect (2, 0, leftBorder - 6, 12), new UIStringAttributes () { ForegroundColor = UIColor.Black, Font = UIFont.SystemFontOfSize (11), ParagraphStyle = paragraphStyle });

			points = new CGPoint [2];
			points [0] = new CGPoint (leftBorder - 3, height + 2 - height / 4);
			points [1] = new CGPoint (leftBorder + 3, height + 2 - height / 4);
			path.AddLines (points);

			points [0] = new CGPoint (leftBorder - 3, height + 2 - height / 2);
			points [1] = new CGPoint (leftBorder + 3, height + 2 - height / 2);
			path.AddLines (points);

			points [0] = new CGPoint (leftBorder - 3, height + 2 - height * 3 / 4);
			points [1] = new CGPoint (leftBorder + 3, height + 2 - height * 3 / 4);
			path.AddLines (points);

			context.AddPath (path);

            context.DrawPath (CGPathDrawingMode.Stroke);

            Console.WriteLine("Redraw in " + (DateTime.Now - t1).TotalMilliseconds + " ms.");
		}

    }
}