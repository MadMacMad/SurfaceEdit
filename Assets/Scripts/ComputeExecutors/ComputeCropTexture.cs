using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tilify
{
    public class ComputeCropTexture : ComputeExecutor<RenderTexture>
    {
        private RenderTexture textureToCrop;
        private Vector2Int cropOrigin;
        private Vector2Int cropSize;

        public ComputeCropTexture (RenderTexture textureToCrop, Vector2Int cropOrigin, Vector2Int cropSize) : base ("Shaders/Compute/CropTexture")
        {
            this.textureToCrop = textureToCrop;
            this.cropOrigin = cropOrigin;
            this.cropSize = cropSize;
        }
        
        public override RenderTexture Execute ()
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (cropSize);

            shader.SetTexture (DefaultFunctionID, "Texture", textureToCrop);
            shader.SetTexture (DefaultFunctionID, "Result", renderTexture);
            shader.SetFloats ("CropOrigin", cropOrigin.x, cropOrigin.y);
            shader.SetFloats ("TextureSize", textureToCrop.width, textureToCrop.height);

            AutoDispatchDefaultShaderFunction (textureToCrop.width, textureToCrop.height, 8, 8);

            return renderTexture;
        }
    }
}
