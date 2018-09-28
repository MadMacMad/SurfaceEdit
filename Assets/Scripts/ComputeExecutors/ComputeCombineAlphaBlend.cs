using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeCombineAlphaBlend : PartTextureComputeExecutor
    {
        public ComputeCombineAlphaBlend(RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture mask) : base(bottomTexture, "Shaders/Compute/AlphaBlend")
        {
            Assert.ArgumentNotNull (mask, nameof (mask));
            Assert.ArgumentNotNull (topTexture, nameof (topTexture));

            Assert.ArgumentTrue (bottomTexture.IsHasEqualSize (topTexture, mask), "Texture sizes are not equal");

            shader.SetTexture (ShaderFunctionID, "BottomTexture", bottomTexture);
            shader.SetTexture (ShaderFunctionID, "TopTexture", topTexture);

            shader.SetTexture (ShaderFunctionID, "Mask", mask);

            shader.SetFloats ("TextureSize", bottomTexture.width, bottomTexture.height);
        }
    }
}
