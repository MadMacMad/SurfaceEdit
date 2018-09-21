using System;
using UnityEngine;

namespace SurfaceEdit.Brushes
{
    public abstract class Brush : PropertyChangedNotifier, IDisposable
    {
        public Vector2 PercentageSize
        {
            get => percentageSize;
            set => SetProperty (ref percentageSize, value, false, v => { v.Clamp (Vector2.zero, new Vector2(float.MaxValue, float.MaxValue)); return v; });
        }
        private Vector2 percentageSize;

        public float PercentageIntervals
        {
            get => percentageIntervals;
            set => SetProperty (ref percentageIntervals, value, false, v => Mathf.Clamp (v, .00001f, 10f));
        }
        private float percentageIntervals;

        public float RealIntervals { get; private set; }

        public Material Material
        {
            get
            {
                if (material == null)
                {
                    material = new Material (Shader.Find ("Tilify/Procedural/Brush"));
                    Update ();
                }
                return material;
            }
        }
        private Material material;

        public RenderTexture BrushStamp
        {
            get => brushStamp;
            set => SetProperty (ref brushStamp, value);
        }
        private RenderTexture brushStamp;

        protected Brush (Vector2 percentageSize, float percentageIntervals)
        {
            percentageSize.Clamp(Vector2.zero, new Vector2(float.MaxValue, float.MaxValue));
            percentageIntervals = Mathf.Clamp (percentageIntervals, .00001f, 10f);

            this.percentageSize = percentageSize;
            this.percentageIntervals = percentageIntervals;

            Update ();
            PropertyChanged += Update;
        }

        private void Update(object sender = null, EventArgs eventArgs = null)
        {
            RealIntervals = percentageIntervals * Mathf.Max (percentageSize.x, percentageSize.y);
            Material.SetFloat ("_QuadScaleX", percentageSize.x);
            Material.SetFloat ("_QuadScaleY", percentageSize.y);
            Material.mainTexture = brushStamp;
        }
        public BrushSnapshot AsSnapshot () => new BrushSnapshot (this);

        public virtual void Dispose ()
        {
            BrushStamp.Release ();
        }
    }
}
