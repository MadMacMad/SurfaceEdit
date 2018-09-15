using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public class ComputeAlphaBlend : ComputeExecutor<UnityEngine.RenderTexture>
    {
        private UnityEngine.RenderTexture topTexture;
        private UnityEngine.RenderTexture bottomTexture;
        private UnityEngine.RenderTexture mask;

        public ComputeAlphaBlend(UnityEngine.RenderTexture topTexture, UnityEngine.RenderTexture bottomTexture, UnityEngine.RenderTexture mask) : base("Shaders/Compute/AlphaBlend")
        {
            this.topTexture = topTexture;
            this.bottomTexture = bottomTexture;
            this.mask = mask;
        }

        public override RenderTexture Execute ()
        {
            var maxTextureSize = new Vector2Int (Mathf.Max (topTexture.width, bottomTexture.width), Mathf.Max (topTexture.height, bottomTexture.height));

            var result = Utils.CreateAndAllocateRenderTexture (maxTextureSize);

            shader.SetTexture (DefaultFunctionID, "Result", result);
            shader.SetTexture (DefaultFunctionID, "BottomTexture", bottomTexture);
            shader.SetTexture (DefaultFunctionID, "TopTexture", topTexture);
            shader.SetTexture (DefaultFunctionID, "Mask", mask);

            shader.SetFloats ("MaxTextureSize", maxTextureSize.x, maxTextureSize.y);

            AutoDispatchDefaultShaderFunction (maxTextureSize.x, maxTextureSize.y, 8, 8);

            return result;
        }
    }
}
