using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tilify.Brushes;
using UnityEngine;

namespace Tilify
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

        public Action<PaintEntry> OnPaint;

        private bool isTriggeredLastFrame;
        private Vector2 lastPosition;
        
        private void Awake ()
        {
            currentBrush = new DefaultRoundBrush (.1f, .1f, 256, .8f);

            PropertyChanged += (s, e) =>
            {
                isTriggeredLastFrame = false;
            };
        }
        private void Update ()
        {
            var triggerEntry = PaintTrigger ();

            if ( triggerEntry.isPaintTriggered )
            {
                var newPosition = triggerEntry.paintPosition;

                // On first trigger always paint.
                if ( !isTriggeredLastFrame )
                {
                    isTriggeredLastFrame = true;
                    lastPosition = newPosition;
                    
                    OnPaint (new PaintEntry (currentBrush.AsSnapshot (), new Vector2[] { newPosition }));
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

                    var paintEntry = new PaintEntry (currentBrush.AsSnapshot (), new Vector2[] { point });
                    OnPaint (paintEntry);
                }
                // Movement is enough to paint several times in a line.
                else
                {
                    var points = DivideLineSegmentIntoPoints ( lastPosition, newPosition, currentBrush.RealIntervals);
                    // lastPosition is set to last point. We ignore the rest of the movement because it is not enough to contain one more point.
                    lastPosition = points.Last();

                    var paintEntry = new PaintEntry (currentBrush.AsSnapshot (), points);
                    OnPaint (paintEntry);
                }
            }
            else
                isTriggeredLastFrame = false;
        }

        private static Vector2[] DivideLineSegmentIntoPoints (Vector2 startPosition, Vector2 endPosition, float distanceBetweenPoints)
        {
            var originVector = endPosition - startPosition;
            var normalizedVector = originVector.normalized;
            var pointOffset = normalizedVector * distanceBetweenPoints;

            // We subtract one because we dont need the first point that will be equals startPosition, because it is already been drawn.
            var pointsCount = Mathf.FloorToInt (originVector.magnitude / distanceBetweenPoints) - 1;
            var points = new Vector2[pointsCount];
            // For the same reason we add pointOffset to startPosition.
            var offsetPosition = startPosition + pointOffset;

            for ( int i = 0; i < pointsCount; i++ )
            {
                points[i] = offsetPosition;
                offsetPosition += pointOffset;
            }

            return points;
        }

        public class PaintTriggerEntry
        {
            public readonly bool isPaintTriggered;
            public readonly Vector2 paintPosition;

            public PaintTriggerEntry (bool isPaintTriggered, Vector2 paintPosition)
            {
                this.isPaintTriggered = isPaintTriggered;
                this.paintPosition = paintPosition;
            }
        }
    }
}
