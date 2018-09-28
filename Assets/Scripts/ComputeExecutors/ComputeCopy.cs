using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeCopy : PartTextureComputeExecutor
    {
        public ComputeCopy (Texture2D source, RenderTexture destination)     : this(source as Texture, destination) { }
        public ComputeCopy (RenderTexture source, RenderTexture destination) : this(source as Texture, destination) { }

        private ComputeCopy(Texture source, RenderTexture destination) : base(destination, "Shaders/Compute/Copy")
        {
            Assert.ArgumentNotNull (source, nameof (source));
            Assert.ArgumentTrue (source.IsHasEqualSize (destination), "Texture sizes are not equals");

            shader.SetTexture (ShaderFunctionID, "Result", destination);
            shader.SetTexture (ShaderFunctionID, "Texture", source);
            shader.SetFloats ("TextureSize", source.width, source.height);
        }
    }
}
