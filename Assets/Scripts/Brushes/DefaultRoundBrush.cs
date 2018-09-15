using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tilify
{
    public class DefaultRoundBrush : Brush
    {
        public float Hardness { get; }

        public DefaultRoundBrush(float realSize, int resolution, float hardness) : base(new Vector2(realSize, realSize))
        {
            if (resolution < 8)
                resolution = 8;
            hardness = Mathf.Clamp01 (hardness);

            Hardness = hardness;

            BrushStamp = new ComputeRoundBrushCreate (resolution, hardness).Execute();
        }
    }
}
