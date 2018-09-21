using UnityEngine;

namespace Tilify.Commands
{
    public class PaintCommand : ICommand
    {
        private RendererStation rendererStation;
        private GameObject go;
        private float objectWidth;

        public PaintCommand (RendererStation rendererStation, GameObject go, float objectWidth)
        {
            Assert.ArgumentNotNull (rendererStation, nameof (rendererStation));
            Assert.ArgumentNotNull (go, nameof (go));
            
            this.rendererStation = rendererStation;
            this.go = go;
            this.objectWidth = Mathf.Clamp(objectWidth, 0, float.MaxValue);
        }

        public void Do ()
        {
            go.SetActive (true);
            rendererStation.UseIt (go, objectWidth);
        }
        public void Undo ()
        {
            rendererStation.StopUseIt (go);
            go.SetActive (false);
        }
        public void Dispose ()
        {
            GameObject.Destroy (go);
        }
    }
}
