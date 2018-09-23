using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SurfaceEdit.Brushes;
using UnityEngine;

namespace SurfaceEdit
{
    public class PaintingManager : UnitySingleton<PaintingManager>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));

        public Brush CurrentBrush
        {
            get => currentBrush;
            set
            {
                if ( value != null )
                {
                    currentBrush.Dispose();
                    currentBrush = value;
                    NotifyPropertyChanged ();
                }
            }
        }
        private Brush currentBrush;

        public Func<PaintTriggerEntry> PaintTrigger;

        public Action<PaintEntry> OnPaintFinal;
        public Action<PaintEntry> OnPaintTemporary;

        private bool isTriggeredLastFrame;
        private Vector2 lastPosition;

        private BrushSnapshot brushSnapshot;
        private List<Vector3> brushPositions = new List<Vector3>();

        private void Awake ()
        {
            currentBrush = new DefaultRoundBrush (new TextureResolution(TextureResolutionEnum.x64), .1f, .1f, .8f);

            PropertyChanged += ResetPaint;
            currentBrush.PropertyChanged += ResetPaint;
        }

        private void ResetPaint (object sender, EventArgs eventArgs)
        {
            isTriggeredLastFrame = false;
        }

        private void Update ()
        {
            var triggerEntry = PaintTrigger ();

            if ( triggerEntry.isActivated )
            {
                if ( !triggerEntry.isPositionValid )
                    return;

                var newPosition = triggerEntry.paintPosition;

                // On first trigger always paint.
                if ( !isTriggeredLastFrame )
                {
                    isTriggeredLastFrame = true;
                    lastPosition = newPosition;

                    brushPositions.Add (newPosition);
                    brushSnapshot = currentBrush.AsSnapshot ();

                    OnPaintTemporary (new PaintEntry (brushSnapshot, brushPositions));
                    return;
                }

                var movement = newPosition - lastPosition;
                var distance = movement.magnitude;

                // If movement is not enough to paint at least once.
                if ( distance < currentBrush.RealIntervals )
                    return;

                // If movement is enough to paint only one time.
                if ( distance < currentBrush.RealIntervals * 2 )
                {
                    // Point belongs to movement vector and it is put from the lastPosition point at a distance equals to the intervals between the brush entries.
                    var point = lastPosition + movement.normalized * currentBrush.RealIntervals;
                    // lastPosition is set to point. We ignore the rest of the movement because it is not enough to contain one more point.
                    lastPosition = point;

                    brushPositions.Add (point);
                    var paintEntry = new PaintEntry (brushSnapshot, brushPositions);
                    OnPaintTemporary (paintEntry);
                }
                // Movement is enough to paint several times in a line.
                else
                {
                    var points = DivideLineSegmentIntoPoints (lastPosition, newPosition, currentBrush.RealIntervals);
                    // lastPosition is set to last point. We ignore the rest of the movement because it is not enough to contain one more point.
                    lastPosition = points.Last ();

                    brushPositions.AddRange (points);

                    var paintEntry = new PaintEntry (brushSnapshot, brushPositions);
                    OnPaintTemporary (paintEntry);
                }
            }
            else
            {
                if ( isTriggeredLastFrame )
                {
                    OnPaintFinal (new PaintEntry (brushSnapshot, brushPositions));
                    brushPositions.Clear ();
                }

                isTriggeredLastFrame = false;
            }
        }

        private static List<Vector3> DivideLineSegmentIntoPoints (Vector2 startPosition, Vector2 endPosition, float distanceBetweenPoints)
        {
            var originVector = endPosition - startPosition;
            var normalizedVector = originVector.normalized;
            var pointOffset = normalizedVector * distanceBetweenPoints;

            // We subtract one because we dont need the first point that will be equals startPosition, because it is already been drawn.
            var pointsCount = Mathf.FloorToInt (originVector.magnitude / distanceBetweenPoints) - 1;
            var points = new List<Vector3>();
            // For the same reason we add pointOffset to startPosition.
            var offsetPosition = startPosition + pointOffset;

            for ( int i = 0; i < pointsCount; i++ )
            {
                points.Add(offsetPosition);
                offsetPosition += pointOffset;
            }

            return points;
        }

        public class PaintTriggerEntry
        {
            public readonly bool isActivated;
            public readonly bool isPositionValid;
            public readonly Vector2 paintPosition;

            public PaintTriggerEntry (bool isActivated, bool isPositionValid, Vector2 paintPosition)
            {
                this.isActivated = isActivated;
                this.isPositionValid = isPositionValid;
                this.paintPosition = paintPosition;
            }
        }
    }
}
