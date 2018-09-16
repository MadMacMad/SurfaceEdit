using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify.TextureAffectors
{
    public class BrightnessTextureAffector : TextureAffector
    {
        public float Factor { get => factor; set => SetPropertyAndRegisterUndoRedo (v => factor = v, () => factor, value, true); }
        private float factor;

        public BrightnessTextureAffector(UndoRedoRegister undoRedoRegister, float factor) : base(undoRedoRegister)
        {
            this.factor = factor;
        }

        public override void Affect (RenderTexture texture)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));
            new ComputeBrighntess (texture, factor).Execute();
        }
    }
}
