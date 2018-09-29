using UnityEngine;
using System.Collections;

namespace SurfaceEdit
{
    [RequireComponent (typeof (Camera))]
    public class CameraRenderLimiter : MonoBehaviour
    {
        public RectTransform provider;

        private Camera cam;

        private void Start ()
        {
            cam = GetComponent<Camera> ();
        }

        private void Update ()
        {
            if (provider != null)
            {
                var viewRectWidth = (Screen.width - provider.sizeDelta.x) / Screen.width;
                cam.rect = new Rect (0, 0, viewRectWidth, 1);
            }
        }
    }
}