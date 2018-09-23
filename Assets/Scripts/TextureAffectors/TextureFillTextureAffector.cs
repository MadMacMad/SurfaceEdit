using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit.TextureAffectors
{
    public class TextureFillTextureAffector : TextureAffector
    {
        private TextureProvider provider;

        public TextureFillTextureAffector (UndoRedoRegister undoRedoRegister, TextureProvider provider) : base (undoRedoRegister)
        {
            Assert.ArgumentNotNull (provider, nameof (provider));

            this.provider = provider;
            provider.NeedUpdate += (s, e) => NotifyNeedUpdate ();
        }

        public override void Affect (RenderTexture texture)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));
            new ComputeCopy (provider.Provide(), texture).Execute ();
        }
    }
}