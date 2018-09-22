﻿using System;
using UnityEngine;

namespace SurfaceEdit.Brushes
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

        public DefaultRoundBrush(TextureResolution resolution, float percentageSize, float intervals, float hardness)
            : base(new Vector2(percentageSize, percentageSize), intervals)
        {
            Assert.ArgumentNotNull (resolution, nameof(resolution));

            this.hardness = Mathf.Clamp01 (hardness);
            Resolution = resolution;

            Resolution.PropertyChanged += (s, e) => UpdateBrushStamp();
            PropertyChanged += (s, e) => UpdateBrushStamp ();
        }
        
        protected override RenderTexture ProvideBrushStamp ()
            => new ComputeRoundBrushCreate (Resolution.Value, hardness).Execute ();
    }
}
