using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeCropTexture : ComputeExecutor<RenderTexture>
    {
        private RenderTexture textureToCrop;
        private Vector2Int cropOrigin;
        private Vector2Int cropSize;

        public ComputeCropTexture (RenderTexture textureToCrop, Vector2Int cropOrigin, Vector2Int cropSize) : base ("Shaders/Compute/CropTexture")
        {
            Assert.ArgumentNotNull (textureToCrop, nameof (textureToCrop));

            cropOrigin.Clamp (new Vector2Int (0, 0), new Vector2Int (textureToCrop.width - 1, textureToCrop.height - 1));
            cropSize.Clamp (new Vector2Int (1, 1), new Vector2Int (textureToCrop.width - cropOrigin.x, textureToCrop.height - cropOrigin.y));

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
