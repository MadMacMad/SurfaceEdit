using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilify.AffectorRenderer;
using UnityEngine;

namespace Tilify.TextureAffectors
{
    public class PaintTextureAffector : TextureAffector
    {
        private AffectorRendererStation rendererStation;

        public PaintTextureAffector(UndoRedoRegister undoRedoRegister) : base (undoRedoRegister)
        {
            rendererStation = new AffectorRendererStation (Settings.affectorRendererStationLayerID);
        }

        public override void Affect (RenderTexture texture)
        {

        }

        public override void Dispose ()
        {
            rendererStation.Dispose ();
        }
    }
}
