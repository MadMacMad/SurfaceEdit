using System;
using UnityEngine;

namespace Tilify.TextureAffectors
{
    public abstract class TextureAffector : PropertyChangedRegistrator, IDisposable
    {
        public TextureAffector(UndoRedoRegister undoRedoRegister) : base (undoRedoRegister) { }

        public abstract void Affect (RenderTexture texture);

        public virtual void Dispose () { }
    }
}
