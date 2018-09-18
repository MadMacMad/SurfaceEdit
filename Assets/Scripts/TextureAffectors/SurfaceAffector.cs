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
    public class SurfaceAffector<Affector> : PropertyChangedRegistrator, ISurfaceAffector where Affector : TextureAffector
    {
        public List<TextureChannel> AffectList { get; private set; } = new List<TextureChannel> ();
        private Affector textureAffector;

        public SurfaceAffector(UndoRedoRegister undoRedoRegister, Affector textureAffector, List<TextureChannel> affectList) : base(undoRedoRegister)
        {
            this.textureAffector = textureAffector;
            AffectList.AddRange(affectList);
            textureAffector.OnNeedUpdate += s => NotifyNeedUpdate (); 
        }

        public void Affect(Surface surface)
        {
            foreach(var pair in surface.SelectTextures (AffectList))
                textureAffector.Affect (pair.Value);
        }
    }
}
