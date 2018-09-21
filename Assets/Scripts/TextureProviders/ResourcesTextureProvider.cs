using UnityEngine;

namespace Tilify.TextureProviders
{
    public class ResourcesTextureProvider : TextureProvider
    {
        private string texturePath;

        public ResourcesTextureProvider (string texturePath, bool cacheTexture = true) : base(cacheTexture)
        {
            this.texturePath = texturePath;
        }
        
        protected override RenderTexture Provide_Internal ()
        {
            var texture = Resources.Load (texturePath) as Texture2D;
            var renderTexture = texture.ConvertToRenderTexture ();
            Resources.UnloadUnusedAssets ();
            return renderTexture;
        }
    }
}
