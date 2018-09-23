using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeRescaleStupid : ComputeExecutor<RenderTexture>
    {
        private RenderTexture texture;
        private Vector2Int adjustedTextureSize;

        public ComputeRescaleStupid (RenderTexture texture, Vector2Int adjustedTextureSize) : base("Shaders/Compute/RescaleStupid")
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            adjustedTextureSize.Clamp (new Vector2Int (2, 2), new Vector2Int (16384, 16384));
            this.adjustedTextureSize = adjustedTextureSize;
            this.texture = texture;
        }

        public override RenderTexture Execute ()
        {
            var result = Utils.CreateAndAllocateRenderTexture (adjustedTextureSize);

            shader.SetTexture (DefaultFunctionID, "Result", result);
            shader.SetTexture (DefaultFunctionID, "Texture", texture);
            shader.SetFloats ("CurrentTextureSize", texture.width, texture.height);
            shader.SetVector ("AdjustedTextureSize", (Vector2)adjustedTextureSize);

            AutoDispatchDefaultShaderFunction (adjustedTextureSize.x, adjustedTextureSize.y);

            return result;
        }
    }
}
