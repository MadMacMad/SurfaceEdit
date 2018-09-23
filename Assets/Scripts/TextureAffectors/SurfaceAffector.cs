using System.Collections.Generic;

namespace SurfaceEdit.TextureAffectors
{
    public class SurfaceAffector : PropertyChangedRegistrator
    {
        public IReadOnlyCollection<TextureChannel> AffectedChannels => affectedChannels.List;
        private TextureChannelCollection affectedChannels;
        private TextureAffector textureAffector;

        public SurfaceAffector(TextureAffector textureAffector, TextureChannelCollection affectedChannels) : base(textureAffector?.undoRedoRegister)
        {
            Assert.ArgumentNotNull (affectedChannels, nameof (affectedChannels));

            this.textureAffector = textureAffector;
            this.affectedChannels = affectedChannels;

            textureAffector.NeedUpdate += (s, e) => NotifyNeedUpdate ();
            affectedChannels.PropertyChanged += (s, e) => NotifyNeedUpdate ();
        }
        
        public void Affect(Surface surface)
        {
            foreach(var pair in surface.SelectTextures (affectedChannels.List) )
                textureAffector.Affect (pair.Value);
        }
    }
}
