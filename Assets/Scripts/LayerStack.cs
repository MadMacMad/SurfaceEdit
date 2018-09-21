using System;
using UnityEngine;

namespace Tilify
{
    public class LayerStack : IDisposable
    {
        private Surface collectorSurface;
        private Surface layerSurface;

        public LayerStack(TextureResolution textureResolution, TextureChannel activeChannels)
        {
            Assert.ArgumentNotNull (textureResolution, nameof (textureResolution));

            collectorSurface = Surface.CreateBlankSurface (textureResolution, activeChannels);
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
