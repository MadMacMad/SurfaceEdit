using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SurfaceEdit
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Toggle))]
    public class ToggleChangeColorsWhenToggled : MonoBehaviour
    {
        public Color colorOn;
        public Color colorOff;

        private Toggle toggle;

        private void Start ()
        {
            toggle = GetComponent<Toggle> ();
            toggle.onValueChanged.AddListener (OnToggleValueChanged);
        }

        private void OnToggleValueChanged (bool isOn)
        {
            var colorBlock = toggle.colors;
            if ( isOn )
            {
                colorBlock.normalColor = colorOn;
                colorBlock.highlightedColor = colorOn;
            }
            else
            {
                colorBlock.normalColor = colorOff;
                colorBlock.highlightedColor = colorOff;
            }
            toggle.colors = colorBlock;
        }
    }
}
