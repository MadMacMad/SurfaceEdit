using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeRescaleStupid : ComputeExecutor<RenderTexture>
    {
        private Vector2Int adjustedTextureSize;

        public ComputeRescaleStupid (RenderTexture texture, Vector2Int adjustedTextureSize) : this(texture as Texture, adjustedTextureSize) { }
        public ComputeRescaleStupid (Texture2D texture, Vector2Int adjustedTextureSize) : this(texture as Texture, adjustedTextureSize) { }

        private ComputeRescaleStupid (Texture texture, Vector2Int adjustedTextureSize) : base ("Shaders/Compute/RescaleStupid")
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            this.adjustedTextureSize = adjustedTextureSize.ClampNew (new Vector2Int (2, 2), new Vector2Int (16384, 16384));

            shader.SetTexture (ShaderFunctionID, "Texture", texture);
            shader.SetFloats ("CurrentTextureSize", texture.width, texture.height);
            shader.SetFloats ("AdjustedTextureSize", this.adjustedTextureSize.x, this.adjustedTextureSize.y);
        }

        public override RenderTexture Execute ()
        {
            var result = TextureUtility.CreateRenderTexture (adjustedTextureSize);

            shader.SetTexture (ShaderFunctionID, "Result", result);

            DispatchShader (adjustedTextureSize.x, adjustedTextureSize.y);

            return result;
        }
    }
}
