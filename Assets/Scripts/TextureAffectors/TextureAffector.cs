using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
