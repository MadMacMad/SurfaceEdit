using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeFillWithColor : ComputeExecutor<RenderTexture>
    {
        private RenderTexture texture;
        private Color color;

        public ComputeFillWithColor (RenderTexture texture, Color color) : base("Shaders/Compute/FillWithColor")
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            this.texture = texture;
            this.color = color;
        }

        public override RenderTexture Execute ()
        {
            shader.SetTexture (DefaultFunctionID, "Result", texture);
            shader.SetFloats ("TextureSize", texture.width, texture.height);
            shader.SetFloats ("Color", color.r, color.g, color.b, color.a);

            AutoDispatchDefaultShaderFunction (texture.width, texture.height);

            return texture;
        }
    }
}
