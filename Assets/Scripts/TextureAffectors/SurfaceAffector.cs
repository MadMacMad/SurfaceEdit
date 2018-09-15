using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilify.TextureAffectors
{
    public interface ISurfaceAffector
    {
        void Affect (Surface surface);
    }
    public class SurfaceAffector<Affector> : ISurfaceAffector where Affector : TextureAffector
    {
        public List<TextureChannel> AffectList { get; private set; } = new List<TextureChannel> ();
        private Affector textureAffector;

        public SurfaceAffector(Affector textureAffector, List<TextureChannel> affectList)
        {
            this.textureAffector = textureAffector;
            AffectList.AddRange(affectList);
        }

        public void Affect(Surface surface)
        {
            foreach(var pair in surface.SelectTextures (AffectList))
                textureAffector.Affect (pair.Value);
        }
    }
}
