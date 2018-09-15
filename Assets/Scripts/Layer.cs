using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tilify.TextureAffectors;
using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Layer : PropertyChangedRegistrator
    {
        public Layer (UndoRedoRegister undoRedoRegister ) : base (undoRedoRegister)
        {

        }
    }
}
