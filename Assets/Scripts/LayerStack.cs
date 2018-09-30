using System;
using System.Collections.Generic;

namespace SurfaceEdit
{
    public sealed class LayerStack : PropertyChangedRegistrator, IDisposable
    {
        public ProgramContext Context { get; private set; }

        public IReadOnlyCollection<Layer> Layers => layers.AsReadOnly ();
        private List<Layer> layers = new List<Layer> ();

        public Surface ResultSurface { get; private set; }
        private Surface layerSurface;
        
        public LayerStack (ProgramContext context)
            : base (context?.UndoRedoRegister)
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
            layer.NeedRender += (s, e) => RequestRender(s, e.renderContext);
            layers.Add (layer);
            RequestRender (this, new RenderContext(Context.Channels.ToImmutable()));
            return layer;
        }
        
        private bool renderRequestedThisFrame = false;

        public void RequestRender (object sender, RenderContext renderContext)
        {
            if ( !renderRequestedThisFrame )
            {
                UnityUpdateRegistrator.Instance.OnUpdateRegisterOneTimeAction (() => RenderImmidiate(renderContext));
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
