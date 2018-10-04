using UnityEngine;

namespace SurfaceEdit.SurfaceAffectors
{
    public sealed class DummySurfaceAffector : SurfaceAffector
    {
        public DummySurfaceAffector(ProgramContext context, Channels affectedChannels) : base(context, affectedChannels) { }

        protected override void Affect (ProviderTexture texture, Vector2Int pixelPosition, Vector2Int pixelSize) { }
    }
}
