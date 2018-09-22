using System;
using UnityEngine;

namespace SurfaceEdit.Brushes
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

            material = new Material (Shader.Find("SurfaceEdit/Procedural/BrushPoints"));
            material.mainTexture = brushStamp;
            material.SetFloat ("_QuadScaleX", percentageSize.x);
            material.SetFloat ("_QuadScaleY", percentageSize.y);
        }

        public void Dispose()
        {
            GameObject.Destroy (material);
        }
    }
}
