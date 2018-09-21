using System.Net;
using UnityEngine;

namespace Tilify.TextureProviders
{
    public class WebTextureProvider : TextureProvider
    {
        private string textureLink;

        public WebTextureProvider (string textureLink, bool cacheTexture = true) : base (cacheTexture)
        {
            this.textureLink = textureLink;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var client = new WebClient ();
            var data = client.DownloadData (textureLink);
            var texture = new Texture2D(2, 2);
            texture.LoadImage (data);
            var renderTexture = texture.ConvertToRenderTexture ();
            Resources.UnloadUnusedAssets ();
            return renderTexture;
        }
    }
}
