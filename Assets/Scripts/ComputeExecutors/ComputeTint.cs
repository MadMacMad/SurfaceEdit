using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeTint : PartTextureComputeExecutor
    {
        public ComputeTint (RenderTexture texture, Color tintColor) : base (texture, "Shaders/Compute/Tint")
        {
            shader.SetTexture (ShaderFunctionID, "Result", texture);
            shader.SetColor ("TintColor", tintColor);
            shader.SetFloats ("TextureSize", texture.width, texture.height);

        }
    }
}
