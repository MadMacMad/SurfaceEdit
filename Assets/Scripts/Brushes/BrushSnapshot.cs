using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify.Brushes
{
    public sealed class BrushSnapshot
    {
        public readonly Brush brush;

        public readonly Vector2 percentageSize;
        public readonly float intervals;

        public readonly RenderTexture brushStamp;

        public BrushSnapshot (Brush brush)
        {
            this.brush = brush;
            percentageSize = brush.PercentageSize;
            intervals = brush.Intervals;
            brushStamp = brush.BrushStamp;
        }

    }
}
