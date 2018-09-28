using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeRoundBrushCreate : ComputeExecutor<RenderTexture>
    {
        private int resolution;

        public ComputeRoundBrushCreate (int resolution, float hardness) : base ("Shaders/Compute/RoundBrushCreate")
        {
            this.resolution = Mathf.Clamp (resolution, 1, 4096);
            hardness = Mathf.Clamp01 (hardness);

            shader.SetFloats ("BrushCenter", this.resolution / 2f, this.resolution / 2f);
            shader.SetFloat ("Resolution", this.resolution);
            shader.SetFloat ("Hardness", hardness);
        }

        public override RenderTexture Execute ()
        {
            var renderTexture = Utils.CreateRenderTexture (resolution);

            shader.SetTexture (ShaderFunctionID, "Result", renderTexture);

            DispatchShader (renderTexture.width, renderTexture.height);
            return renderTexture;
        }
    }
}
