using System;
using UnityEngine;

namespace SurfaceEdit.Commands
{
    public class PaintCommand : ICommand
    {
        private RendererStation rendererStation;
        private GameObject go;
        private float objectWidth;
        private Action onNeedUpdate;

        public PaintCommand (RendererStation rendererStation, GameObject go, float objectWidth, Action onNeedUpdate)
        {
            Assert.ArgumentNotNull (rendererStation, nameof (rendererStation));
            Assert.ArgumentNotNull (go, nameof (go));

            this.rendererStation = rendererStation;
            this.go = go;
            this.objectWidth = Mathf.Clamp(objectWidth, 0, float.MaxValue);
            this.onNeedUpdate = onNeedUpdate;
        }

        public void Do ()
        {
            go.SetActive (true);
            rendererStation.UseIt (go, objectWidth);
            onNeedUpdate?.Invoke ();
        }
        public void Undo ()
        {
            rendererStation.StopUseIt (go);
            go.SetActive (false);
            onNeedUpdate?.Invoke ();
        }
        public void Dispose ()
        {
            GameObject.Destroy (go);
        }
    }
}
