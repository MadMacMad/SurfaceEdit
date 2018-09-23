using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit.TextureAffectors
{
    public class ColorFillTextureAffector : TextureAffector
    {
        private Color color;

        public ColorFillTextureAffector(UndoRedoRegister undoRedoRegister, Color color) : base(undoRedoRegister)
        {
            this.color = color;
        }

        public override void Affect (RenderTexture texture)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));
            new ComputeFillWithColor (texture, color).Execute ();
        }
    }
}