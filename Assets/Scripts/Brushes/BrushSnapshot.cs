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
        public readonly Vector2 percentageSize;
        public readonly float intervals;

        public readonly RenderTexture brushStamp;

        public BrushSnapshot (Vector2 percentageSize, float intervals, RenderTexture brushStamp)
        {
            percentageSize.Clamp01 ();
            if ( intervals <= 0 )
                intervals = .001f;

            this.percentageSize = percentageSize;
            this.intervals = intervals;
            this.brushStamp = brushStamp;
        }

    }
}
