using UnityEngine;

namespace SurfaceEdit.Affectors
{
    public sealed class DummyAffector : Affector
    {
        public DummyAffector(ApplicationContext context, Channels affectedChannels) : base(context, affectedChannels) { }

        protected override void Affect (ProviderTexture texture, Vector2Int pixelPosition, Vector2Int pixelSize) { }
    }
}
