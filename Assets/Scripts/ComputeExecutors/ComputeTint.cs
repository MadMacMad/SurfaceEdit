using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeTint : ComputeExecutor<RenderTexture>
    {
        private RenderTexture texture;
        private Color tintColor;

        public ComputeTint (RenderTexture texture, Color tintColor) : base ("Shaders/Compute/Tint")
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            this.texture = texture;
            this.tintColor = tintColor;
        }

        public override RenderTexture Execute ()
        {
            shader.SetTexture (DefaultFunctionID, "Result", texture);
            shader.SetColor ("TintColor", tintColor);
            shader.SetFloats ("TextureSize", texture.width, texture.height);

            AutoDispatchDefaultShaderFunction (texture.width, texture.height);

            return texture;
        }
    }
}
