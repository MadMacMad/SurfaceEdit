using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace SurfaceEdit
{
    // https://forum.unity.com/threads/can-the-ui-buttons-detect-a-right-mouse-click.279027/

    public class MouseClickEventSender : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnLeftClicked;
        public UnityEvent OnMiddleClicked;
        public UnityEvent OnRightClicked;

        public void OnPointerClick (PointerEventData eventData)
        {
            if ( eventData.button == PointerEventData.InputButton.Left )
                OnLeftClicked?.Invoke ();
            else if ( eventData.button == PointerEventData.InputButton.Middle )
                OnMiddleClicked?.Invoke ();
            else if ( eventData.button == PointerEventData.InputButton.Right )
                OnRightClicked?.Invoke ();
        }
    }
}