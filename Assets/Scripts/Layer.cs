using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tilify.TextureAffectors;
using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Layer : ObjectChangedRegistrator
    {
        private List<ISurfaceAffector> surfaceAffectors;

        public Layer (UndoRedoRegister undoRedoRegister ) : base (undoRedoRegister)
        {

        }

        public void AddSurfaceAffector<T>(SurfaceAffector<T> affector) where T : TextureAffector
        {
        }
    }
}
