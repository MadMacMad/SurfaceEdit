using System;
using UnityEngine;

namespace SurfaceEdit.TextureAffectors
{
    public abstract class TextureAffector : PropertyChangedRegistrator, IDisposable
    {
        public TextureAffector(UndoRedoRegister undoRedoRegister) : base (undoRedoRegister) { }

        public abstract void Affect (RenderTexture texture);

        public virtual void Dispose () { }
    }
}
