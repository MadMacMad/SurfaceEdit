using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class LayerStack : PropertyChangedRegistrator, IDisposable
    {
        public TextureResolution Resolution { get; private set; }
        public TextureChannelCollection Channels { get; private set; }

        public IReadOnlyCollection<Layer> Layers => layers.AsReadOnly ();
        private List<Layer> layers = new List<Layer> ();

        public Surface ResultSurface => collectorSurface;

        private Surface collectorSurface;
        private Surface layerSurface;
        
        public LayerStack (UndoRedoRegister undoRedoRegister, TextureResolution resolution, TextureChannelCollection channels)
            : base (undoRedoRegister)
        {
            Assert.ArgumentNotNull (resolution, nameof (resolution));
            Assert.ArgumentNotNull (channels, nameof (channels));

            Resolution = resolution;
            Channels = channels;

            collectorSurface = new Surface (resolution, Channels);
            layerSurface = new Surface (resolution, Channels);
            
            Channels.PropertyChanged += Update;
        }

        public Layer CreateLayer ()
        {
            var layer = new Layer (undoRedoRegister);
            layer.NeedUpdate += Update;
            layers.Add (layer);
            RequestRender ();
            return layer;
        }

        private void Update (object sender = null, EventArgs args = null)
        {
            RequestRender ();
        }

        private bool renderRequestedThisFrame = false;

        public void RequestRender ()
        {
            if ( !renderRequestedThisFrame )
            {
                UnityUpdateRegistrator.Instance.OnUpdateRegisterOneTimeAction (RenderImmidiate);
                renderRequestedThisFrame = true;
            }
        }
        public void RenderImmidiate()
        {
            renderRequestedThisFrame = false;
            collectorSurface.ResetAll ();
            int index = 0;
            foreach ( var layer in Layers )
            {
                if ( index == 0 )
                    layer.Process (collectorSurface);
                else
                {
                    layerSurface.ResetAll ();
                    layer.Process (layerSurface);
                    SurfaceCombiner.CombineSurfaces (collectorSurface, layerSurface, layer.BlendType);
                }
                index++;
            }
        }

        public void Dispose ()
        {
            collectorSurface.Dispose ();
            layerSurface.Dispose ();
        }
    }
}
