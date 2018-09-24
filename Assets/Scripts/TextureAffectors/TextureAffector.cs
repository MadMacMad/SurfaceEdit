using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit.TextureAffectors
{
    public class NeedRenderEventArgs : EventArgs
    {
        public readonly IReadOnlyCollection<Vector2Int> ChunkPositions;

        public NeedRenderEventArgs (List<Vector2Int> chunkPositions)
        {
            ChunkPositions = chunkPositions.AsReadOnly ();
        }
    }

    public delegate void NeedRenderEventHandler (object sender, EventArgs eventArgs);


    public abstract class TextureAffector : PropertyChangedRegistrator, IDisposable
    {
        public event NeedRenderEventHandler NeedRender;

        public TextureAffector(UndoRedoRegister undoRedoRegister) : base (undoRedoRegister) { }

        public abstract void Affect (RenderTexture texture);

        public SurfaceAffector ToSurfaceAffector(TextureChannelCollection channels)
            => new SurfaceAffector ( this, channels);

        public SurfaceAffector ToSurfaceAffector (TextureChannel channel)
            => new SurfaceAffector (this, new TextureChannelCollection(channel));

        public virtual void Dispose () { }
    }
}
