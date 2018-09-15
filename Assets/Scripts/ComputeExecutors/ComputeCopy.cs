using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public class ComputeCopy : ComputeExecutor<RenderTexture>
    {
        private Texture sourceTexture;
        private RenderTexture destinationTexture;

        public ComputeCopy (Texture2D source, RenderTexture destination) : base("Shaders/Compute/Copy")
        {
            sourceTexture = source;
            destinationTexture = destination;
        }

        public ComputeCopy (RenderTexture source, RenderTexture destination) : base ("Shaders/Compute/Copy")
        {
            sourceTexture = source;
            destinationTexture = destination;
        }

        public override RenderTexture Execute ()
        {
            shader.SetTexture (DefaultFunctionID, "Result", destinationTexture);
            shader.SetTexture (DefaultFunctionID, "Texture", sourceTexture);
            shader.SetFloats ("TextureSize", sourceTexture.width, sourceTexture.height);

            AutoDispatchDefaultShaderFunction (sourceTexture.width, sourceTexture.height, 8, 8);

            return destinationTexture;
        }
    }
}
