using UnityEngine;

namespace Tilify
{
    public class ComputeRoundBrushCreate : ComputeExecutor<RenderTexture>
    {
        private int resolution;
        private float hardnessFactor;

        public ComputeRoundBrushCreate (int resolution, float hardnessFactor) : base ("Shaders/Compute/RoundBrushCreate")
        {
            resolution = Mathf.Clamp (resolution, 1, 4096);
            hardnessFactor = Mathf.Clamp01 (hardnessFactor);

            this.resolution = resolution;
            this.hardnessFactor = hardnessFactor;
        }

        public override RenderTexture Execute ()
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (resolution);

            shader.SetTexture (DefaultFunctionID, "Result", renderTexture);
            shader.SetFloats ("BrushCenter", resolution / 2f, resolution / 2f);
            shader.SetFloat ("Size", resolution);
            shader.SetFloat ("Hardness", hardnessFactor);

            AutoDispatchDefaultShaderFunction (renderTexture.width, renderTexture.height, 8, 8);
            
            return renderTexture;
        }
    }
}
