using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tilify.Brushes
{
    public class DefaultRoundBrush : Brush
    {
        public float Hardness { get; }

        public DefaultRoundBrush(float percentageSize, float intervals, int resolution, float hardness)
            : base(new Vector2(percentageSize, percentageSize), intervals)
        {
            resolution = Mathf.Clamp (resolution, 1, 4096);
            hardness = Mathf.Clamp01 (hardness);

            Hardness = hardness;

            BrushStamp = new ComputeRoundBrushCreate (resolution, hardness).Execute();
        }
    }
}
