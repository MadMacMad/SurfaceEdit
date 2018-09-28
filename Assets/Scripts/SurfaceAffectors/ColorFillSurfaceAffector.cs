using UnityEngine;

namespace SurfaceEdit.SurfaceAffectors
{
    public sealed class ColorFillSurfaceAffector : SurfaceAffector
    {
        private Color color;

        public ColorFillSurfaceAffector (ProgramContext context, Channels affectedChannels, Color color)
            : base (context, affectedChannels)
        {
            this.color = color;
        }

        private ComputeFillWithColor compute;

        protected override void PreAffect (ProviderTexture chunkTexture)
        { 
            compute = new ComputeFillWithColor (chunkTexture.RenderTexture, color);
        }

        protected override void PostAffect ()
            => compute = null;

        protected override void Affect (ProviderTexture chunkTexture, Vector2Int pixelPosition, Vector2Int pixelSize)
        {
            compute.Origin = pixelPosition;
            compute.Size = pixelSize;
            compute.Execute ();
        }
    }
}