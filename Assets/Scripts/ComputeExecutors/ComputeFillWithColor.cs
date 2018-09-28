using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeFillWithColor : PartTextureComputeExecutor
    {
        public ComputeFillWithColor (RenderTexture texture, Color color) : base(texture, "Shaders/Compute/FillWithColor")
        {
            shader.SetTexture (ShaderFunctionID, "Result", texture);
            shader.SetFloats ("TextureSize", texture.width, texture.height);
            shader.SetColor ("Color", color);
        }
    }
}
