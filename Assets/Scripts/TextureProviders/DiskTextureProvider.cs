using UnityEngine;

namespace Tilify.TextureProviders
{
    public class DiskTextureProvider : TextureProvider
    {
        private string texturePath;

        public DiskTextureProvider (string texturePath, bool cacheTexture = true) : base (cacheTexture)
        {
            this.texturePath = texturePath;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var texture = TextureIOHelper.LoadTexture2DFromDisk (texturePath);
            var renderTexture = texture.ConvertToRenderTexture ();
            Resources.UnloadUnusedAssets ();
            return renderTexture;
        }
    }
}
