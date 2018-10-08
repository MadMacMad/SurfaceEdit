using System;
using System.Collections.Generic;

namespace SurfaceEdit
{
    public sealed class LayerStack : PropertyChangedRegistrator, IDisposable
    {
        public ApplicationContext Context { get; private set; }

        public event Action<Layer> OnLayerCreate;
        public event Action<Layer> OnLayerDelete;

        public IReadOnlyCollection<Layer> Layers => layers.AsReadOnly ();
        private List<Layer> layers = new List<Layer> ();

        public Surface ResultSurface { get; private set; }
        private Surface layerSurface;

        private bool renderRequestedThisFrame = false;

        public LayerStack (ApplicationContext context)
            : base (context?.UndoRedoManager)
        {
            Assert.ArgumentNotNull (context, nameof (context));

            Context = context;

            ResultSurface = new Surface (context);
            layerSurface = new Surface (context);
            
            Context.Changed += (s, e) => RequestRender(this, new RenderContext(Context.Channels.ToImmutable(), RenderCovering.Full));
        }

        public Layer CreateLayer ()
        {
            var layer = new Layer (Context);
            layer.NeedRender += RequestRenderByLayer;
            layers.Add (layer);
            RequestRender (this, new RenderContext(Context.Channels.ToImmutable()));
            OnLayerCreate?.Invoke (layer);
            return layer;
        }
        
        public void DeleteLayer(Layer layer)
        {
            if ( layer == null )
                return;

            if ( !layers.Contains (layer) )
                return;

            layer.NeedRender -= RequestRenderByLayer;
            layer.Dispose ();

            layers.Remove (layer);

            RequestRender (this, new RenderContext (Context.Channels.ToImmutable ()));
            OnLayerDelete?.Invoke (layer);
        }

        private void RequestRenderByLayer (object sender, NeedRenderEventArgs eventArgs)
            => RequestRender (sender, eventArgs.renderContext);
        

        public void RequestRender (object sender, RenderContext renderContext)
        {
            if ( !renderRequestedThisFrame )
            {
                UnityCallbackRegistrator.Instance.RegisterOneTimeUpdateAction (() => RenderImmidiate(renderContext));
                renderRequestedThisFrame = true;
            }
        }
        public void RenderImmidiate(RenderContext renderContext)
        {
            renderRequestedThisFrame = false;
            ResultSurface.Reset (renderContext);
            int index = 0;
            foreach ( var layer in Layers )
            {
                if ( index == 0 )
                    layer.Process (ResultSurface, renderContext);
                else
                {
                    layerSurface.Reset (renderContext);
                    layer.Process (layerSurface, renderContext);

                    SurfaceCombiner.CombineSurfaces (Context, renderContext, ResultSurface, layerSurface, layer.BlendType);
                }
                index++;
            }
        }

        public void Dispose ()
        {
            ResultSurface.Dispose ();
            layerSurface.Dispose ();
        }
    }
}
