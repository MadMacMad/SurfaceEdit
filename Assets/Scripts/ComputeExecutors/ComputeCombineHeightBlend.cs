using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeCombineHeightBlend : ComputeExecutor<RenderTexture>
    {
        private RenderTexture topTexture;
        private RenderTexture bottomTexture;
        private RenderTexture topHeight;
        private RenderTexture bottomHeight;
        private RenderTexture mask;

        public ComputeCombineHeightBlend (RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture bottomHeight, RenderTexture topHeight, RenderTexture mask)
            : base("Shaders/Compute/HeightBlend")
        {
            Assert.ArgumentNotNull (mask, nameof (mask));
            Assert.ArgumentNotNull (topTexture, nameof (topTexture));
            Assert.ArgumentNotNull (bottomTexture, nameof (bottomTexture));
            Assert.ArgumentNotNull (bottomHeight, nameof (bottomHeight));
            Assert.ArgumentNotNull (topHeight, nameof (topHeight));

            this.topTexture = topTexture;
            this.bottomTexture = bottomTexture;
            this.bottomHeight = bottomHeight;
            this.topHeight = topHeight;
            this.mask = mask;
        }

        public override RenderTexture Execute ()
        {
            var maxTextureSize = new Vector2Int (Mathf.Max (topTexture.width, bottomTexture.width, bottomHeight.width, topHeight.width),
                                                 Mathf.Max (topTexture.height, bottomTexture.height, bottomHeight.height, topHeight.height));
            
            shader.SetTexture (DefaultFunctionID, "BottomTexture", bottomTexture);
            shader.SetTexture (DefaultFunctionID, "TopTexture", topTexture);
            shader.SetTexture (DefaultFunctionID, "BottomTextureHeight", bottomHeight);
            shader.SetTexture (DefaultFunctionID, "TopTextureHeight", topHeight);

            shader.SetTexture (DefaultFunctionID, "Mask", mask);

            shader.SetFloats ("MaxTextureSize", maxTextureSize.x, maxTextureSize.y);
            shader.SetFloats ("BottomTextureHeightFactor", 1);
            shader.SetFloats ("TopTextureHeightFactor", 1);
            shader.SetFloats ("DepthFactor", .3f);
            shader.SetBool ("IsHeight", false);

            AutoDispatchDefaultShaderFunction (maxTextureSize.x, maxTextureSize.y);

            return bottomTexture;
        }
    }
}
