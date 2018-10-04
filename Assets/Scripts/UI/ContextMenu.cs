using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace SurfaceEdit
{
    public class ContextMenu : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        public event Action OnDestroy;

        private bool hasFocus = false;

        private void Update ()
        {
            if ( !hasFocus )
                if ( Input.anyKeyDown )
                {
                    OnDestroy?.Invoke ();
                    GameObject.Destroy (this);
                }
        }

        public void OnPointerExit (PointerEventData eventData)
            => hasFocus = false;
        
        public void OnPointerEnter (PointerEventData eventData)
            => hasFocus = true;
        
    }
}