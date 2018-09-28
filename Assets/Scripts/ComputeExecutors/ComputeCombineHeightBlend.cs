using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeCombineHeightBlend : PartTextureComputeExecutor
    {
        public ComputeCombineHeightBlend (RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture bottomHeight, RenderTexture topHeight, RenderTexture mask)
            : base(bottomTexture, "Shaders/Compute/HeightBlend")
        {
            Assert.ArgumentNotNull (mask, nameof (mask));
            Assert.ArgumentNotNull (topTexture, nameof (topTexture));
            Assert.ArgumentNotNull (bottomHeight, nameof (bottomHeight));
            Assert.ArgumentNotNull (topHeight, nameof (topHeight));

            Assert.ArgumentTrue (bottomTexture.IsHasEqualSize (topTexture, bottomHeight, topHeight, mask), "Texture sizes are not equal");

            shader.SetTexture (ShaderFunctionID, "BottomTexture", bottomTexture);
            shader.SetTexture (ShaderFunctionID, "TopTexture", topTexture);
            shader.SetTexture (ShaderFunctionID, "BottomTextureHeight", bottomHeight);
            shader.SetTexture (ShaderFunctionID, "TopTextureHeight", topHeight);
            shader.SetTexture (ShaderFunctionID, "Mask", mask);

            shader.SetFloats ("MaxTextureSize", bottomHeight.width, bottomHeight.height);
            shader.SetFloats ("BottomTextureHeightFactor", 1);
            shader.SetFloats ("TopTextureHeightFactor", 1);
            shader.SetFloats ("DepthFactor", .3f);
            shader.SetBool ("IsHeight", false);
        }
    }
}
