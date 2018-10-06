using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public class DiskTextureProvider : TextureProvider
    {
        private string texturePath;

        public DiskTextureProvider (TextureResolution resolution, string texturePath) : base (resolution)
        {
            this.texturePath = texturePath;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var texture = TextureUtility.LoadTexture2DFromDisk (texturePath);
            var renderTexture = texture.ConvertToRenderTexture ();
            Resources.UnloadUnusedAssets ();
            return renderTexture;
        }
    }
}
