using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public class ResourcesTextureProvider : TextureProvider
    {
        private string texturePath;

        public ResourcesTextureProvider (TextureResolution resolution, string texturePath, bool cacheTexture = true) : base(resolution, cacheTexture)
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
