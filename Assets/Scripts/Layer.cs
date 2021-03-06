﻿using System;
using System.Collections.Generic;
using SurfaceEdit.Affectors;

namespace SurfaceEdit
{
    public class Layer : PropertyChangedRegistrator, INotifyNeedRender, IDisposable
    {
        public event NeedRenderEventHandler NeedRender;

        protected void NotifyNeedRender (RenderContext renderContext)
            => NeedRender?.Invoke (this, new NeedRenderEventArgs(renderContext));
     
        public string ID { get; private set; }

        public ApplicationContext Context { get; private set; }

        public LayerBlendType BlendType
        {
            get => blendType;
            set => SetPropertyUndoRedo (v => blendType = v, () => blendType, value, true);
        }
        private LayerBlendType blendType = LayerBlendType.HeightBlend;

        public IReadOnlyCollection<Channel> Channels => channels.AsReadOnly();
        private List<Channel> channels = new List<Channel>();

        public IReadOnlyCollection<Affector> Affectors => affectors.AsReadOnly();
        private List<Affector> affectors = new List<Affector> ();
        
        public Layer (ApplicationContext context) : base (context?.UndoRedoManager)
        {
            Assert.ArgumentNotNull (context, nameof (context));

            Context = context;

            ID = Guid.NewGuid ().ToString ();
        }
        
        public void Process(Surface surface, RenderContext renderContext)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));
            Assert.ArgumentNotNull (renderContext, nameof (renderContext));
            
            foreach ( var affector in affectors )
                affector.AffectSurface (surface, renderContext);
        }
        
        public void AddAffector(Affector affector)
        {
            if (!affectors.Contains(affector))
            {
                affector.NeedRender += OnAffectorNeedRender;
                affectors.Add (affector);
                NotifyNeedRender (new RenderContext(affector.AffectedChannels.ToImmutable(), RenderCovering.Full));
            }
        }
        public void RemoveAffector (Affector affector) 
        {
            if ( affectors.Contains (affector) )
            {
                affector.NeedRender -= OnAffectorNeedRender;
                affectors.Remove(affector);
                affector.Dispose ();
                NotifyNeedRender (new RenderContext(affector.AffectedChannels.ToImmutable(), RenderCovering.Full));
            }
        }

        public void Reset()
        {
            foreach(var affector in affectors)
            {
                affector.NeedRender -= OnAffectorNeedRender;
                affector.Dispose ();
            }
            affectors.Clear ();
            NotifyNeedRender (new RenderContext (Context.Channels.ToImmutable (), RenderCovering.Full));
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
