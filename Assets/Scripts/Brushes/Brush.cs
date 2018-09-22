using System;
using UnityEngine;

namespace SurfaceEdit.Brushes
{
    public abstract class Brush : PropertyChangedNotifier, IDisposable
    {
        public Vector2 PercentageSize
        {
            get => percentageSize;
            set => SetProperty (ref percentageSize, value, false, v => { v.Clamp (Vector2.zero, new Vector2 (float.MaxValue, float.MaxValue)); return v; });
        }
        private Vector2 percentageSize;

        public float PercentageIntervals
        {
            get => percentageIntervals;
            set => SetProperty (ref percentageIntervals, value, false, v => Mathf.Clamp (v, .00001f, 10f));
        }
        private float percentageIntervals;

        public RenderTexture BrushStamp { get; private set; }

        public float RealIntervals { get; private set; }

        protected Brush (Vector2 percentageSize, float percentageIntervals)
        {
            percentageSize.Clamp (Vector2.zero, new Vector2 (float.MaxValue, float.MaxValue));
            percentageIntervals = Mathf.Clamp (percentageIntervals, .00001f, 10f);

            this.percentageSize = percentageSize;
            this.percentageIntervals = percentageIntervals;

            BrushStamp = ProvideBrushStamp ();

            PropertyChanged += (s, e) =>
            {
                if ( e?.propertyName == "PercentageSize" || e?.propertyName == "PercentageIntervals" )
                    RealIntervals = percentageIntervals * Mathf.Max (percentageSize.x, percentageSize.y);
            };
        }

        public void UpdateBrushStamp ()
        {
            BrushStamp = ProvideBrushStamp ();
            NotifyPropertyChanged ("BrushStamp");
        }
        public BrushSnapshot AsSnapshot () => new BrushSnapshot (this);

        public void Dispose ()
        {
            BrushStamp.Release ();
            Dispose_Internal ();
        }
        protected virtual void Dispose_Internal () { }
        protected abstract RenderTexture ProvideBrushStamp ();
    }
}
