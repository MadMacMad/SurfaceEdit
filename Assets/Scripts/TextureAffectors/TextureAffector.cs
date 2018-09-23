using System;
using UnityEngine;

namespace SurfaceEdit.TextureAffectors
{
    public abstract class TextureAffector : PropertyChangedRegistrator, IDisposable
    {
        public TextureAffector(UndoRedoRegister undoRedoRegister) : base (undoRedoRegister) { }

        public abstract void Affect (RenderTexture texture);

        public SurfaceAffector ToSurfaceAffector(TextureChannelCollection channels)
            => new SurfaceAffector ( this, channels);

        public SurfaceAffector ToSurfaceAffector (TextureChannel channel)
            => new SurfaceAffector (this, new TextureChannelCollection(channel));

        public virtual void Dispose () { }
    }
}
