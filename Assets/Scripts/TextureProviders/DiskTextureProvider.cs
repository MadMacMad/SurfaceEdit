﻿using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public class DiskTextureProvider : TextureProvider
    {
        private string texturePath;

        public DiskTextureProvider (TextureResolution resolution, string texturePath, bool cacheTexture = true) : base (resolution, cacheTexture)
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
