using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tilify.Brushes
{
    public abstract class Brush : PropertyChangedNotifier, IDisposable
    {
        public Vector2 PercentageSize
        {
            get => percentageSize;
            set => SetProperty (ref percentageSize, value, false, v => { v.Clamp01 (); return v; });
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
                    material = new Material (Shader.Find ("Tilify/Unlit/Transparent"))
                    {
                        enableInstancing = true,
                        mainTexture = BrushStamp
                    };
                }
                return material;
            }
        }
        private Material material;

        public RenderTexture BrushStamp { get; protected set; }

        protected Brush (Vector2 percentageSize, float percentageIntervals)
        {
            percentageSize.Clamp01 ();
            percentageIntervals = Mathf.Clamp (percentageIntervals, .00001f, 10f);

            this.percentageSize = percentageSize;
            this.percentageIntervals = percentageIntervals;

            UpdateRealIntervals (null, null);
            PropertyChanged += UpdateRealIntervals;
        }

        private void UpdateRealIntervals(object sender, EventArgs eventArgs)
        {
            RealIntervals = percentageIntervals * Mathf.Max (percentageSize.x, percentageSize.y);
        }
        public BrushSnapshot AsSnapshot () => new BrushSnapshot (this);

        public virtual void Dispose ()
        {
            BrushStamp.Release ();
        }
    }
}
