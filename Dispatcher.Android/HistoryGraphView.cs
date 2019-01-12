using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using DataUtils;

namespace Dispatcher.Android
{
    public class HistoryGraphView : View
    {
        private class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        {
            public float Scale { get; set; } = 1;

            public override bool OnScale(ScaleGestureDetector detector)
            {
                Scale *= detector.ScaleFactor;                    
                return true;
            }
        }

        float leftBorder;
        float rightBorder;
        float downBorder;

        private List<HistoryPoint> historyList;
        public DateTime timeStart, timeEnd;
        public bool readyToDataUpdate = false;

        private ScaleGestureDetector _scaleDetector;
        private ScaleListener _scaleListener;

        public HistoryGraphView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public HistoryGraphView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            _scaleListener = new ScaleListener();
            _scaleDetector = new ScaleGestureDetector(Context, _scaleListener);

            leftBorder = ConvertSize(30);
            rightBorder = ConvertSize(10);
            downBorder = ConvertSize(40);
        }

        private float lastTouchX;
        private float currentTouchX;       

        DateTime initialPanTimeStart;
        DateTime initialPinchTimeStart, initialPinchTimeEnd;

        double relativePos;

        public override bool OnTouchEvent(MotionEvent e)
        {
            _scaleDetector.OnTouchEvent(e);

            switch (e.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    readyToDataUpdate = false;
                    initialPanTimeStart = timeStart;
                    initialPinchTimeStart = timeStart;
                    initialPinchTimeEnd = timeEnd;
                    lastTouchX = e.GetX();
                    relativePos = e.GetX() / Width;
                    currentTouchX = 0;
                    _scaleListener.Scale = 1;                    
                    break;
                case MotionEventActions.Move:
                    if (!_scaleDetector.IsInProgress)
                    {
                        currentTouchX = e.GetX();
                        double relativeX = (currentTouchX - lastTouchX) / Width;
                        double frame = (timeEnd - timeStart).TotalSeconds;
                        timeStart = initialPanTimeStart.AddSeconds(-frame * relativeX);
                        timeEnd = timeStart.AddSeconds(frame);
                    }
                    else
                    {
                        double oldFrame = (initialPinchTimeEnd - initialPinchTimeStart).TotalSeconds;
                        double newFrame = oldFrame / _scaleListener.Scale;
                        timeStart = initialPinchTimeStart.AddSeconds((oldFrame - newFrame) * relativePos);
                        timeEnd = timeStart.AddSeconds(newFrame);                        
                    }
                    Invalidate();
                    break;
                case MotionEventActions.Up:                                        
                    readyToDataUpdate = true;                    
                    break;
            }

            return true;
        }

        public void SetData(List<HistoryPoint> historyList, DateTime timeStart, DateTime timeEnd)
        {
            this.historyList = historyList;
            this.timeStart = timeStart;
            this.timeEnd = timeEnd;
        }

        double initialMinValue = double.MaxValue;
        double initialMaxValue = double.MinValue;

        private float ConvertSize(float size)
        {
            float scaledSizeInPixels = size * Resources.DisplayMetrics.ScaledDensity;
            return scaledSizeInPixels;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var paint = new Paint(PaintFlags.AntiAlias)
            {
                Color = Color.Black,
                StrokeWidth = ConvertSize(1)                
            };

            paint.SetStyle(Paint.Style.Stroke);            

            var path = new Path();

            float width = Width - leftBorder - rightBorder;
            float height = Height - downBorder;

            path.Reset();
            path.MoveTo(leftBorder - ConvertSize(3), ConvertSize(3));
            path.LineTo(leftBorder, 0);
            path.LineTo(leftBorder + ConvertSize(3), ConvertSize(3));            
            path.MoveTo(leftBorder, 0);
            path.LineTo(leftBorder, height + ConvertSize(2));
            path.LineTo(leftBorder + width + rightBorder / 2, height + ConvertSize(2));
            path.LineTo(leftBorder + width + rightBorder / 2 - ConvertSize(3), height + ConvertSize(2) - ConvertSize(3));
            path.LineTo(leftBorder + width + rightBorder / 2, height + ConvertSize(2));
            path.LineTo(leftBorder + width + rightBorder / 2 - ConvertSize(3), height + ConvertSize(2) + ConvertSize(3));

            canvas.DrawPath(path, paint);

            var topLinePaint = new TextPaint(PaintFlags.AntiAlias) { Color = Color.Black, TextSize = ConvertSize(12) };
            var bottomLinePaint = new TextPaint(PaintFlags.AntiAlias) { Color = Color.Black, TextSize = ConvertSize(7) };

            float topShift1 = 17;
            float topShift2 = 29;
            
            canvas.DrawText(timeStart.ToString("HH:mm"), leftBorder, height + ConvertSize(topShift1), topLinePaint);
            canvas.DrawText(timeStart.ToString("dd.MM.yy"), leftBorder, height + ConvertSize(topShift2), bottomLinePaint);
            canvas.DrawText(timeEnd.ToString("HH:mm"), leftBorder + width - ConvertSize(25), height + ConvertSize(topShift1), topLinePaint);
            canvas.DrawText(timeEnd.ToString("dd.MM.yy"), leftBorder + width - ConvertSize(25), height + ConvertSize(topShift2), bottomLinePaint);

            if (historyList == null || !historyList.Any()) return;

            var graphPaint = new Paint(PaintFlags.AntiAlias)
            {
                Color = Color.ParseColor("#a7a7a7"),
                StrokeWidth = ConvertSize(2)
            };

            graphPaint.SetStyle(Paint.Style.Stroke);

            float period = (timeEnd - timeStart).Ticks;

            double minValue = initialMinValue;
            double maxValue = initialMaxValue;
            for (int s = 0; s < historyList.Count; s++)
            {
                if (historyList[s].value < minValue)
                    minValue = historyList[s].value;
                if (historyList[s].value > maxValue)
                    maxValue = historyList[s].value;
            }

            initialMinValue = minValue;
            initialMaxValue = maxValue;

            double valueRange = maxValue - minValue;

            path.Reset();

            int pointX = 0;

            bool needToMove = true;

            for (int s = 0; s < historyList.Count; s++)
            {
                if ((historyList[s].time < timeStart) || 
                    (historyList[s].time > timeEnd))
                    continue;

                float X = leftBorder + (historyList[s].time - timeStart).Ticks / period * width;
                float Y = (float)(height + 2 - ((historyList[s].value - minValue) / valueRange * height));

                if (pointX == (int)width) break;               

                if (needToMove)
                {
                    path.MoveTo(X, Y);
                    needToMove = false;
                }                    
                else
                    path.LineTo(X, Y);

                pointX++;                
            }

            canvas.DrawPath(path, graphPaint);

            // X axis
            DateTime quartDate = timeStart + new TimeSpan((timeEnd - timeStart).Ticks / 4);
            DateTime mediumDate = timeStart + new TimeSpan((timeEnd - timeStart).Ticks / 2);
            DateTime threeQuartDate = timeStart + new TimeSpan(((timeEnd - timeStart).Ticks * 3 / 4));

            canvas.DrawText(quartDate.ToString("HH:mm"), leftBorder + width / 4 - ConvertSize(15), height + ConvertSize(topShift1), topLinePaint);
            canvas.DrawText(quartDate.ToString("dd.MM.yy"), leftBorder + width / 4 - ConvertSize(15), height + ConvertSize(topShift2), bottomLinePaint);
            canvas.DrawText(mediumDate.ToString("HH:mm"), leftBorder + width / 2 - ConvertSize(15), height + ConvertSize(topShift1), topLinePaint);
            canvas.DrawText(mediumDate.ToString("dd.MM.yy"), leftBorder + width / 2 - ConvertSize(15), height + ConvertSize(topShift2), bottomLinePaint);
            canvas.DrawText(threeQuartDate.ToString("HH:mm"), leftBorder + width * 3 / 4 - ConvertSize(15), height + ConvertSize(topShift1), topLinePaint);
            canvas.DrawText(threeQuartDate.ToString("dd.MM.yy"), leftBorder + width * 3 / 4 - ConvertSize(15), height + ConvertSize(topShift2), bottomLinePaint);

            path.Reset();
            path.MoveTo(leftBorder + width / 4, height + ConvertSize(2) - ConvertSize(3));
            path.LineTo(leftBorder + width / 4, height + ConvertSize(2) + ConvertSize(3));

            path.MoveTo(leftBorder + width / 2, height + ConvertSize(2) - ConvertSize(3));
            path.LineTo(leftBorder + width / 2, height + ConvertSize(2) + ConvertSize(3));

            path.MoveTo(leftBorder + width * 3 / 4, height + ConvertSize(2) - ConvertSize(3));
            path.LineTo(leftBorder + width * 3 / 4, height + ConvertSize(2) + ConvertSize(3));

            canvas.DrawPath(path, paint);

            // Y axis

            double quartValue = minValue + ((maxValue - minValue) / 4);
            double mediumValue = minValue + ((maxValue - minValue) / 2);
            double threeQuartValue = minValue + ((maxValue - minValue) * 3 / 4);

            var yPaint = new TextPaint(PaintFlags.AntiAlias)
            {
                Color = Color.Black,
                TextSize = ConvertSize(11),
                TextAlign = Paint.Align.Right
            };

            var borderOffset = leftBorder - ConvertSize(5);

            canvas.DrawText(minValue.ToString("F1"), borderOffset, height, yPaint);
            canvas.DrawText(quartValue.ToString("F1"), borderOffset, height + ConvertSize(6) - height / 4, yPaint);
            canvas.DrawText(mediumValue.ToString("F1"), borderOffset, height + ConvertSize(6) - height / 2, yPaint);
            canvas.DrawText(threeQuartValue.ToString("F1"), borderOffset, height + ConvertSize(6) - height * 3 / 4, yPaint);
            canvas.DrawText(maxValue.ToString("F1"), borderOffset, ConvertSize(12), yPaint);

            path.Reset();
            path.MoveTo(leftBorder - ConvertSize(3), height + ConvertSize(2) - height / 4);
            path.LineTo(leftBorder + ConvertSize(3), height + ConvertSize(2) - height / 4);

            path.MoveTo(leftBorder - ConvertSize(3), height + ConvertSize(2) - height / 2);
            path.LineTo(leftBorder + ConvertSize(3), height + ConvertSize(2) - height / 2);

            path.MoveTo(leftBorder - ConvertSize(3), height + ConvertSize(2) - height * 3 / 4);
            path.LineTo(leftBorder + ConvertSize(3), height + ConvertSize(2) - height * 3 / 4);

            canvas.DrawPath(path, paint);
        }
    }
}