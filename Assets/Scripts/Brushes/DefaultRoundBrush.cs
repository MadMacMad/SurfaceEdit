using UnityEngine;

namespace Tilify.Brushes
{
    public class DefaultRoundBrush : Brush
    {
        public float Hardness
        {
            get => hardness;
            set => SetProperty (ref hardness, value, true, v => Mathf.Clamp01 (v));
        }
        private float hardness;

        public int Resolution
        {
            get => resolution;
            set => SetProperty (ref resolution, value, true, v => Mathf.Clamp (v, 1, 4096));
        }
        private int resolution;

        public DefaultRoundBrush(float percentageSize, float intervals, int resolution, float hardness)
            : base(new Vector2(percentageSize, percentageSize), intervals)
        {
            this.resolution = Mathf.Clamp (resolution, 1, 4096);
            this.hardness = Mathf.Clamp01 (hardness);
            
            NeedUpdate += OnNeedUpdate;
            OnNeedUpdate (this);
        }

        private void OnNeedUpdate(object sender)
        {
            BrushStamp = new ComputeRoundBrushCreate (resolution, hardness).Execute ();
        }
    }
}
