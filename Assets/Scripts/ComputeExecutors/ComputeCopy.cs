using UnityEngine;

namespace SurfaceEdit
{
    public class ComputeCopy : ComputeExecutor<RenderTexture>
    {
        private Texture sourceTexture;
        private RenderTexture destinationTexture;

        public ComputeCopy (Texture2D source, RenderTexture destination) : base("Shaders/Compute/Copy")
        {
            Assert.ArgumentNotNull (source, nameof (source));
            Assert.ArgumentNotNull (destination, nameof (destination));

            sourceTexture = source;
            destinationTexture = destination;
        }

        public ComputeCopy (RenderTexture source, RenderTexture destination) : base ("Shaders/Compute/Copy")
        {
            Assert.ArgumentNotNull (source, nameof (source));
            Assert.ArgumentNotNull (destination, nameof (destination));

            sourceTexture = source;
            destinationTexture = destination;
        }

        public override RenderTexture Execute ()
        {
            if ( sourceTexture == destinationTexture )
                return destinationTexture;

            shader.SetTexture (DefaultFunctionID, "Result", destinationTexture);
            shader.SetTexture (DefaultFunctionID, "Texture", sourceTexture);
            shader.SetFloats ("TextureSize", sourceTexture.width, sourceTexture.height);

            AutoDispatchDefaultShaderFunction (sourceTexture.width, sourceTexture.height);

            return destinationTexture;
        }
    }
}
