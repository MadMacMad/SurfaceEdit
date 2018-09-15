using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilify.TextureAffectors
{
    public class TextureStackAffector<Affector> where Affector : TextureAffector
    {
        public List<TextureChannel> AffectList { get; private set; } = new List<TextureChannel> ();
        private Affector textureAffector;

        public TextureStackAffector(Affector textureAffector, List<TextureChannel> affectList)
        {
            this.textureAffector = textureAffector;
            AffectList.AddRange(affectList);
        }

        public void Affect(TextureStack stack)
        {
            foreach(var pair in stack.SelectTextures (AffectList))
                textureAffector.Affect (pair.Value);
        }
    }
}
