using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tilify
{
    public static class Settings
    {
        public static readonly Vector2Int minTextureSize = new Vector2Int(64, 64);
        public static readonly Vector2Int maxTextureSize = new Vector2Int(8192, 8192);
        public static readonly int affectorRendererStationLayerID = LayerMask.NameToLayer ("AffectorRendererStation");
    }
}
