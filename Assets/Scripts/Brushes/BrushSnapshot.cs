using System;
using UnityEngine;

namespace Tilify.Brushes
{
    public sealed class BrushSnapshot : IDisposable
    {
        public readonly Material material;
        public readonly Vector2 percentageSize;
        public readonly float intervals;

        public readonly RenderTexture brushStamp;

        public BrushSnapshot (Brush brush)
        {
            percentageSize = brush.PercentageSize;
            intervals = brush.PercentageIntervals;
            brushStamp = brush.BrushStamp;
            material = new Material (brush.Material);
        }

        public void Dispose()
        {
            GameObject.Destroy (material);
        }
    }
}
