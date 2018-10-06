using System;
using System.Linq;
using UnityEngine;

namespace SurfaceEdit.Affectors
{
    public abstract class Affector : PropertyChangedRegistrator, IDisposable
    {
        public event NeedRenderEventHandler NeedRender;

        protected void NotifyNeedRender(RenderContext renderContext)
            => NeedRender?.Invoke (this, new NeedRenderEventArgs(renderContext));

        public ApplicationContext Context { get; private set; }

        public Channels AffectedChannels { get; private set; }

        public Affector (ApplicationContext context, Channels affectedChannels) : base (context?.UndoRedoManager)
        {
            Assert.ArgumentNotNull (context, nameof (context));
            Assert.ArgumentNotNull (affectedChannels, nameof (affectedChannels));

            Context = context;
            AffectedChannels = affectedChannels;

            AffectedChannels.Changed += (s, e) => NotifyNeedRender (new RenderContext(AffectedChannels.ToImmutable()));
        }

        public void AffectSurface (Surface surface, RenderContext renderContext)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));
            Assert.ArgumentNotNull (renderContext, nameof (renderContext));

            if ( renderContext.Covering == RenderCovering.Part )
            {
                foreach ( var pair in surface.SelectTextures (AffectedChannels.List) )
                {
                    if ( !renderContext.ChannelsToRender.List.Contains (pair.Key) )
                        continue;

                    var texture = pair.Value;

                    PreAffect (texture);

                    foreach ( var pixelPosition in renderContext.ChunksToRender.PixelPositions )
                        Affect (texture, pixelPosition, Context.ChunkResolution.AsVector);

                    PostAffect ();
                }
            }
            else
            {
                foreach ( var pair in surface.SelectTextures (AffectedChannels.List) )
                {
                    if ( !renderContext.ChannelsToRender.List.Contains (pair.Key) )
                        continue;

                    PreAffect (pair.Value);

                    Affect (pair.Value, Vector2Int.zero, Context.TextureResolution.AsVector);

                    PostAffect ();
                }
            }
        }
        
        protected abstract void Affect (ProviderTexture texture, Vector2Int pixelPosition, Vector2Int pixelSize);

        protected virtual void PreAffect (ProviderTexture texture) { }
        protected virtual void PostAffect () { }
        
        public virtual void Dispose () { }
    }
}