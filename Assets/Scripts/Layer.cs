using System;
using System.Collections.Generic;
using SurfaceEdit.SurfaceAffectors;
using UnityEngine;

namespace SurfaceEdit
{
    public class Layer : PropertyChangedRegistrator, INotifyNeedRender, IDisposable
    {
        public event NeedRenderEventHandler NeedRender;

        protected void NotifyNeedRender (RenderContext renderContext)
            => NeedRender?.Invoke (this, new NeedRenderEventArgs(renderContext));
        
        public ProgramContext Context { get; private set; }

        public LayerBlendType BlendType
        {
            get => blendType;
            set => SetPropertyUndoRedo (v => blendType = v, () => blendType, value, true);
        }
        private LayerBlendType blendType = LayerBlendType.HeightBlend;

        public IReadOnlyCollection<Channel> Channels => channels.AsReadOnly();
        private List<Channel> channels = new List<Channel>();

        public IReadOnlyCollection<SurfaceAffector> Affectors => affectors.AsReadOnly();
        private List<SurfaceAffector> affectors = new List<SurfaceAffector> ();
        
        public Layer (ProgramContext context) : base (context?.UndoRedoRegister)
        {
            Assert.ArgumentNotNull (context, nameof (context));

            Context = context;
        }
        
        public void Process(Surface surface, RenderContext renderContext)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));
            Assert.ArgumentNotNull (renderContext, nameof (renderContext));
            
            foreach ( var affector in affectors )
                affector.AffectSurface (surface, renderContext);
        }
        
        public void AddAffector(SurfaceAffector affector)
        {
            if (!affectors.Contains(affector))
            {
                affector.NeedRender += OnAffectorNeedRender;
                affectors.Add (affector);
                NotifyNeedRender (new RenderContext(affector.AffectedChannels.ToImmutable(), RenderCovering.Full));
            }
        }
        public void RemoveAffector (SurfaceAffector affector) 
        {
            if ( affectors.Contains (affector) )
            {
                affector.NeedRender -= OnAffectorNeedRender;
                affectors.Remove(affector);
                NotifyNeedRender (new RenderContext(affector.AffectedChannels.ToImmutable(), RenderCovering.Full));
            }
        }

        private void OnAffectorNeedRender (object sender, NeedRenderEventArgs eventArgs)
            => NotifyNeedRender (eventArgs.renderContext);

        public void Dispose()
        {

        }
    }
    public enum LayerBlendType
    {
        AlphaBlend,
        HeightBlend
    }
}
