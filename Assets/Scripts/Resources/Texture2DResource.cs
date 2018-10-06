using System;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class Texture2DResource : Resource
    {
        public Texture2D Texture { get; private set; }
        public override Texture2D PreviewTexture { get; protected set; }

        public Texture2DResource (string name, string diskPathToResource, Texture2D texture, ApplicationContext context) : base (name, diskPathToResource, context)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            Texture = texture;
            PreviewTexture = new ComputeRescaleStupid (Texture, new Vector2Int (128, 128)).Execute ().ConvertToTexture2DAndRelease ();
            AdjustScale ();

            context.TextureResolution.Changed += (s, e) => AdjustScale();
        }
            
        private void AdjustScale()
        {
            if ( Texture.width != Context.TextureResolution.AsInt || Texture.height != Context.TextureResolution.AsInt )
            {
                var result = new ComputeRescaleStupid (Texture, Context.TextureResolution.AsVector).Execute ();
                GameObject.DestroyImmediate (Texture);
                Texture = result.ConvertToTexture2DAndRelease ();
            }
        }

        protected override void Dispose_Internal ()
        {
            GameObject.DestroyImmediate (Texture);
        }
    }
}