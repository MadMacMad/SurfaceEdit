using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilify.Brushes;
using UnityEngine;

namespace Tilify.TextureAffectors
{
    public class PaintTextureAffector : TextureAffector
    {
        private static readonly Mesh quadMesh;

        static PaintTextureAffector()
        {
            quadMesh = MeshBuilder.BuildQuad (Vector2.one).ConvertToMesh ();
        }

        private RendererStation rendererStation;

        public PaintTextureAffector(UndoRedoRegister undoRedoRegister) : base (undoRedoRegister)
        {
            rendererStation = new RendererStation (Settings.affectorRendererStationLayerID);
        }

        public void Paint(PaintEntry paintEntry)
        {
            var material = paintEntry.brushSnapshot.brush.Material;
                
            foreach ( var position in paintEntry.BrushPositions )
            {
                var go = new GameObject ("Brush");
                var renderer = go.AddComponent<MeshRenderer> ();

                go.AddComponent<MeshFilter> ().mesh = quadMesh;
                renderer.material = material;
                go.transform.position = position;
                go.transform.localScale = paintEntry.brushSnapshot.percentageSize;

                rendererStation.UseIt (go);
            }
        }

        public override void Affect (RenderTexture texture)
        {
            rendererStation.Render (texture);
        }

        public override void Dispose ()
        {
            rendererStation.Dispose ();
        }
    }
}
