using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SurfaceEdit
{
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(RectTransform))]
    public class BindToggleToToggle : MonoBehaviour
    {
        public Toggle otherToggle;
        private Toggle toggle;
        
        private void Start ()
        {
            toggle = GetComponent<Toggle> ();
            toggle.onValueChanged.AddListener (OnToggle);
            OnToggle (toggle.isOn);
        }
        
        private void OnToggle(bool value)
        {
            if ( otherToggle != null )
                otherToggle.isOn = value;
        }
    }
}