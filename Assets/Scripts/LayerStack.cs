﻿using System;
using UnityEngine;

namespace Tilify
{
    public class LayerStack : IDisposable
    {
        private Surface collectorSurface;
        private Surface layerSurface;

        public LayerStack(Vector2Int textureSize, TextureChannel activeChannels)
        {
            textureSize = TextureHelper.Instance.ClampTextureSize (textureSize);

            collectorSurface = Surface.CreateBlankSurface (textureSize, activeChannels);
        }

        public void CreateLayer()
        {

        }

        private void PropertyChangedHandler (object sender, PropertyChangedEventArgs args)
        {
            Render ();
        }
        public void Render ()
        {

        }

        public void Dispose ()
        {

        }
    }
}
