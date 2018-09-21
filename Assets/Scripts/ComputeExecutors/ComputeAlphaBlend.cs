using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeAlphaBlend : ComputeExecutor<RenderTexture>
    {
        private RenderTexture topTexture;
        private RenderTexture bottomTexture;
        private RenderTexture mask;

        public ComputeAlphaBlend(RenderTexture topTexture, RenderTexture bottomTexture, RenderTexture mask) : base("Shaders/Compute/AlphaBlend")
        {
            Assert.ArgumentNotNull (topTexture, nameof (topTexture));
            Assert.ArgumentNotNull (bottomTexture, nameof (bottomTexture));
            Assert.ArgumentNotNull (mask, nameof (mask));

            this.topTexture = topTexture;
            this.bottomTexture = bottomTexture;
            this.mask = mask;
        }

        public override RenderTexture Execute ()
        {
            var maxTextureSize = new Vector2Int (Mathf.Max (topTexture.width, bottomTexture.width), Mathf.Max (topTexture.height, bottomTexture.height));

            var result = Utils.CreateAndAllocateRenderTexture (maxTextureSize);

            shader.SetTexture (DefaultFunctionID, "Result", result);
            shader.SetTexture (DefaultFunctionID, "BottomTexture", bottomTexture);
            shader.SetTexture (DefaultFunctionID, "TopTexture", topTexture);
            shader.SetTexture (DefaultFunctionID, "Mask", mask);

            shader.SetFloats ("MaxTextureSize", maxTextureSize.x, maxTextureSize.y);

            AutoDispatchDefaultShaderFunction (maxTextureSize.x, maxTextureSize.y, 8, 8);

            return result;
        }
    }
}
