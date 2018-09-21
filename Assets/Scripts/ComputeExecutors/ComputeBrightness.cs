using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeBrighntess : ComputeExecutor<RenderTexture>
    {
        private RenderTexture texture;
        private float brightnessFactor;

        public ComputeBrighntess (RenderTexture texture, float brightnessFactor) : base("Shaders/Compute/Brightness")
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            this.texture = texture;
            this.brightnessFactor = brightnessFactor;
        }

        public override RenderTexture Execute ()
        {
            shader.SetTexture (DefaultFunctionID, "Result", texture);
            shader.SetFloats ("TextureSize", texture.width, texture.height);
            shader.SetFloat ("Brightness", brightnessFactor);

            AutoDispatchDefaultShaderFunction (texture.width, texture.height, 8, 8);

            return texture;
        }
    }
}
