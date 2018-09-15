using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tilify
{
    public class ImageBrush : Brush
    {
        public ImageBrush(Vector2 realSize, RenderTexture texture) : base(realSize)
        {
            BrushStamp = texture;
        }
    }
}
