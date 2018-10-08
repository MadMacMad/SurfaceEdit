using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SurfaceEdit.Brushes;
using UnityEngine;

namespace SurfaceEdit
{
    public class PaintingManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));

        public Brush Brush
        {
            get => brush;
            set
            {
                if ( value != null )
                {
                    brush.Dispose();
                    brush = value;
                    NotifyPropertyChanged ();
                }
            }
        }
        private Brush brush;
        
        public event Action<PaintEntry> OnPaintFinal;
        public event Action<PaintEntry> OnPaintTemporary;
        
        private bool isTriggeredLastFrame;
        private Vector2 lastPosition;

        private BrushSnapshot brushSnapshot;

        private PaintEntry temporaryPaintEntry;

        public PaintingManager() : this(new DefaultRoundBrush(new TextureResolution(TextureResolutionEnum.x128), .05f, .25f, 0)) { }
        public PaintingManager(Brush brush)
        {
            Assert.ArgumentNotNull (brush, nameof (brush));
            this.brush = brush;

            PropertyChanged += ResetPaint;
            this.brush.PropertyChanged += ResetPaint;
        }

        private void ResetPaint (object sender, EventArgs eventArgs)
        {
            isTriggeredLastFrame = false;
        }

        public void PaintTriggered (Vector2 paintPosition)
        {
            var newPosition = paintPosition - brush.PercentageSize / 2;

            // On first trigger always paint.
            if ( !isTriggeredLastFrame )
            {
                isTriggeredLastFrame = true;
                lastPosition = newPosition;
                    
                brushSnapshot = brush.AsSnapshot ();

                temporaryPaintEntry = new PaintEntry (brushSnapshot, new List<Vector3> () { newPosition });
                OnPaintTemporary?.Invoke (temporaryPaintEntry);
                return;
            }

            var movement = newPosition - lastPosition;
            var distance = movement.magnitude;

            // If movement is not enough to paint at least once.
            if ( distance < brush.RealIntervals )
                return;

            // If movement is enough to paint only one time.
            if ( distance < brush.RealIntervals * 2 )
            {
                // Point belongs to movement vector and it is put from the lastPosition point at a distance equals to the intervals between the brush entries.
                var point = lastPosition + movement.normalized * brush.RealIntervals;
                // lastPosition is set to point. We ignore the rest of the movement because it is not enough to contain one more point.
                lastPosition = point;
                    
                temporaryPaintEntry.AddBrushPositions (new List<Vector3> () { point });
            }
            // Movement is enough to paint several times in a line.
            else
            {
                var points = DivideLineSegmentIntoPoints (lastPosition, newPosition, brush.RealIntervals);
                // lastPosition is set to last point. We ignore the rest of the movement because it is not enough to contain one more point.
                lastPosition = points.Last ();
                temporaryPaintEntry.AddBrushPositions (points);
            }
        }
        public void PaintNotTriggered()
        {
            if ( isTriggeredLastFrame )
                OnPaintFinal?.Invoke(temporaryPaintEntry);

            isTriggeredLastFrame = false;
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
