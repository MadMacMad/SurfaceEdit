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

        private bool firstTime = true;

        public PaintCommand (RendererStation rendererStation, GameObject go, float objectWidth, Action onNeedUpdate)
        {
            Assert.ArgumentNotNull (rendererStation, nameof (rendererStation));
            Assert.ArgumentNotNull (go, nameof (go));

            this.rendererStation = rendererStation;
            this.go = go;
            this.objectWidth = Mathf.Clamp(objectWidth, 0, float.MaxValue);
            this.onNeedUpdate = onNeedUpdate;
            rendererStation.UseIt (go, objectWidth);
        }

        public void Do ()
        {
            if (firstTime)
            {
                firstTime = false;
                return;
            }
            go.SetActive (true);
            rendererStation.UseIt (go, objectWidth);
            onNeedUpdate?.Invoke ();
        }
        public void Undo ()
        {
            firstTime = false;
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
