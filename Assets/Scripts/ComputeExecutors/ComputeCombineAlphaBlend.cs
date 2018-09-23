using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeCombineAlphaBlend : ComputeExecutor<RenderTexture>
    {
        private RenderTexture topTexture;
        private RenderTexture bottomTexture;
        private RenderTexture mask;

        public ComputeCombineAlphaBlend(RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture mask) : base("Shaders/Compute/AlphaBlend")
        {
            Assert.ArgumentNotNull (mask, nameof (mask));
            Assert.ArgumentNotNull (topTexture, nameof (topTexture));
            Assert.ArgumentNotNull (bottomTexture, nameof (bottomTexture));

            this.topTexture = topTexture;
            this.bottomTexture = bottomTexture;
            this.mask = mask;
        }

        public override RenderTexture Execute ()
        {
            var maxTextureSize = new Vector2Int (Mathf.Max (topTexture.width, bottomTexture.width), Mathf.Max (topTexture.height, bottomTexture.height));

            shader.SetTexture (DefaultFunctionID, "BottomTexture", bottomTexture);
            shader.SetTexture (DefaultFunctionID, "TopTexture", topTexture);

            shader.SetTexture (DefaultFunctionID, "Mask", mask);

            shader.SetFloats ("MaxTextureSize", maxTextureSize.x, maxTextureSize.y);

            AutoDispatchDefaultShaderFunction (maxTextureSize.x, maxTextureSize.y);

            return bottomTexture;
        }
    }
}
