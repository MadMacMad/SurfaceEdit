using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SurfaceEdit
{
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(RectTransform))]
    public class HideOnToggle : MonoBehaviour
    {
        public List<GameObject> hideOnToggle;
        private Toggle toggle;
        
        private void Start ()
        {
            toggle = GetComponent<Toggle> ();
            toggle.onValueChanged.AddListener (OnToggle);
            OnToggle (toggle.isOn);
        }

        private void OnToggle(bool value)
        {
            if ( hideOnToggle != null )
                foreach ( var go in hideOnToggle )
                    go?.SetActive (toggle.isOn);
        }
    }
}
