using UnityEngine;

namespace SurfaceEdit
{
    [ExecuteInEditMode]
    [RequireComponent (typeof (RectTransform))]
    public class CopyRectTransform : MonoBehaviour
    {
        public bool copyWidth;
        public bool copyHeight;
        public RectTransform other;

        private Vector2 lastSize;
        private RectTransform rectTransform;

        private void OnEnable ()
        {
            rectTransform = GetComponent<RectTransform> ();
        }

        private void Update ()
        {
            if ( other != null )
            {
                var otherSize = other.sizeDelta;

                if ( copyWidth && lastSize.x != otherSize.x )
                    rectTransform.sizeDelta = new Vector2 (otherSize.x, rectTransform.sizeDelta.y);

                if ( copyHeight && lastSize.y != otherSize.y )
                    rectTransform.sizeDelta = new Vector2 (rectTransform.sizeDelta.y, otherSize.y);

                lastSize = rectTransform.sizeDelta;
            }
        }
    }
}
