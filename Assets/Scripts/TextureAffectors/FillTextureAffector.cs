using UnityEngine;

namespace Tilify.TextureAffectors
{
    public class FillTextureAffector : TextureAffector
    {
        private Color color;

        public FillTextureAffector(UndoRedoRegister undoRedoRegister, Color color) : base(undoRedoRegister)
        {
            this.color = color;
        }

        public override void Affect (RenderTexture texture)
        {
            new ComputeFillWithColor (texture, color).Execute ();
        }
    }
}
