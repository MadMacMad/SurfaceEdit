﻿using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace SurfaceEdit
{
    public class ContextMenuDestroyer : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
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