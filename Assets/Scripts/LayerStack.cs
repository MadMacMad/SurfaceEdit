using System;
using System.Collections.Generic;

namespace SurfaceEdit
{
    public sealed class LayerStack : PropertyChangedNotifier, IDisposable
    {
        public TextureResolution Resolution { get; private set; }
        public TextureChannelCollection Channels { get; private set; }

        public IReadOnlyCollection<Layer> Layers => layers.AsReadOnly();
        private List<Layer> layers = new List<Layer> ();

        private Surface collectorSurface;
        private Surface layerSurface;

        public LayerStack(TextureResolution textureResolution, TextureChannelCollection channels)
        {
            Assert.ArgumentNotNull (textureResolution, nameof (textureResolution));
            Assert.ArgumentNotNull (channels, nameof (channels));

            Resolution = textureResolution;
            Channels = channels;
            
            collectorSurface = new Surface (textureResolution, Channels);
            layerSurface = new Surface (textureResolution, Channels);


            textureResolution.PropertyChanged += Update;
            Channels.PropertyChanged += Update;
        }

        public void AddLayer()
        {

        }

        private void Update (object sender = null, EventArgs args = null)
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
