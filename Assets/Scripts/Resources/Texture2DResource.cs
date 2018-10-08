using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace SurfaceEdit
{
    public sealed class Texture2DResource : Resource
    {
        [JsonIgnore]
        public Texture2D Texture { get; private set; }
        [JsonIgnore]
        public override Texture2D PreviewTexture { get; protected set; }

        public Texture2DResource (string name, string cacheDirectory, Texture2D texture, ApplicationContext context) : base (name, cacheDirectory, context)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            Texture = texture;
            PreviewTexture = new ComputeRescaleStupid (Texture, new Vector2Int (128, 128)).Execute ().ConvertToTexture2DAndRelease ();
            AdjustScale ();

            context.TextureResolution.Changed += (s, e) => AdjustScale();

            Cache ();
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

        public override void Dispose ()
        {
            base.Dispose ();
            GameObject.DestroyImmediate (Texture);
        }

        protected override void Cache_Child (string cacheDirectory)
        {
            var texturePath = Path.Combine (cacheDirectory, "texture.tga");
            TextureUtility.SaveTexture2DToDisk (texturePath, Texture);
        }
    }
}