using System;
using UnityEngine;

namespace SurfaceEdit.Brushes
{
    public abstract class Brush : PropertyChangedNotifier, IDisposable
    {
        public Vector2 PercentageSize
        {
            get => percentageSize;
            set => SetProperty (ref percentageSize, value, false, v => v.Clamp01New ());
        }
        private Vector2 percentageSize;

        public float PercentageIntervals
        {
            get => percentageIntervals;
            set => SetProperty (ref percentageIntervals, value, false, v => Mathf.Clamp (v, .00001f, 10f));
        }
        private float percentageIntervals;

        public RenderTexture BrushStamp
        {
            get
            {
                if ( brushStamp == null )
                    brushStamp = ProvideBrushStamp ();
                return brushStamp;
            }
        }
        private RenderTexture brushStamp;

        public Color TintColor
        {
            get => tintColor;
            set => SetProperty (ref tintColor, value);
        }
        private Color tintColor = Color.white;

        public float RealIntervals { get; private set; }

        protected Brush (Vector2 percentageSize, float percentageIntervals)
        {
            percentageSize.ClampNew (Vector2.zero, new Vector2 (float.MaxValue, float.MaxValue));
            percentageIntervals = Mathf.Clamp (percentageIntervals, .00001f, 10f);

            this.percentageSize = percentageSize;
            this.percentageIntervals = percentageIntervals;

            RealIntervals = percentageIntervals * Mathf.Max (percentageSize.x, percentageSize.y);

            PropertyChanged += (s, e) =>
            {
                if ( e?.propertyName == "PercentageSize" || e?.propertyName == "PercentageIntervals" )
                    RealIntervals = percentageIntervals * Mathf.Max (percentageSize.x, percentageSize.y);
                if ( e?.propertyName == "TintColor" )
                    UpdateBrushStamp ();
            };
        }

        public void UpdateBrushStamp ()
        {
            brushStamp = ProvideBrushStamp ();
            new ComputeTint (BrushStamp, tintColor).Execute();
            NotifyPropertyChanged ("BrushStamp");
        }
        public BrushSnapshot AsSnapshot () => new BrushSnapshot (this);

        public void Dispose ()
        {
            brushStamp?.Release ();
            Dispose_Internal ();
        }
        protected virtual void Dispose_Internal () { }
        protected abstract RenderTexture ProvideBrushStamp ();
    }
}
