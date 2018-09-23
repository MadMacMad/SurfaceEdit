﻿using SurfaceEdit.Commands;
using UnityEngine;

namespace SurfaceEdit.TextureAffectors
{
    public class PaintTextureAffector : TextureAffector
    {
        private static readonly float distanceBetweenBrushes = .00001f;

        private RendererStation rendererStation;

        public PaintTextureAffector(UndoRedoRegister undoRedoRegister) : base (undoRedoRegister)
        {
            rendererStation = new RendererStation (Settings.affectorRendererStationLayerID);
        }

        public void PaintTemporary(PaintEntry paintEntry) => Paint_Internal (paintEntry, true);
        public void Paint (PaintEntry paintEntry) => Paint_Internal (paintEntry, false);

        private void Paint_Internal(PaintEntry paintEntry, bool isTemporary)
        {
            Assert.ArgumentNotNull (paintEntry, nameof (paintEntry));

            var material = paintEntry.brushSnapshot.material;

            var go = new GameObject ("PaintEntry");
            go.AddComponent<MeshRenderer> ().material = material;
            go.AddComponent<MeshFilter> ().mesh = paintEntry.Mesh;

            var objectWidth = paintEntry.BrushPositions.Count * distanceBetweenBrushes;

            if ( isTemporary )
            {
                rendererStation.UseItTemporary (go, objectWidth);
                NotifyNeedUpdate ();
            }
            else
            {
                var command = new PaintCommand (rendererStation, go, objectWidth, () => NotifyNeedUpdate ());
                undoRedoRegister.Do (command);
            }
        }

        public void Reset()
        {
            rendererStation.Reset ();
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
