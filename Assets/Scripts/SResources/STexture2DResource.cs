using UnityEngine;

namespace SurfaceEdit
{
    public sealed class STexture2DResource : SResource
    {
        public Texture2D Texture { get; private set; }
        public override Texture2D PreviewTexture { get; protected set; }

        public STexture2DResource (string name, Texture2D texture) : base (name)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            Texture = texture;
            PreviewTexture = new ComputeRescaleStupid (texture, new Vector2Int (128, 128)).Execute ().ConvertToTextureAndRelease ();
        }

        public override void Dispose ()
        {
            GameObject.DestroyImmediate (Texture);
            GameObject.DestroyImmediate (PreviewTexture);
        }
    }
}