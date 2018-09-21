using System;

namespace SurfaceEdit
{
    public sealed class LayerStack : PropertyChangedNotifier, IDisposable
    {
        public TextureResolution Resolution { get; private set; }
        public TextureChannelCollection Channels { get; private set; }

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

        public void CreateLayer()
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
