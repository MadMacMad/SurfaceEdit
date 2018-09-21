using System;
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

        public TextureResolution Resolution { get; private set; }

        public DefaultRoundBrush(TextureResolution textureResolution, float percentageSize, float intervals, float hardness)
            : base(new Vector2(percentageSize, percentageSize), intervals)
        {
            this.hardness = Mathf.Clamp01 (hardness);

            Resolution.PropertyChanged += OnNeedUpdate;
        }

        private void OnNeedUpdate(object sender, EventArgs eventArgs)
        {
            BrushStamp = new ComputeRoundBrushCreate (Resolution.Value, hardness).Execute ();
        }
    }
}
