using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SurfaceEdit
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Toggle))]
    public class ToggleAdvancedAnimations : MonoBehaviour
    {
        public GameObject hideOnToggle;

        [Space(10)]
        public string normalOn = "NormalOn";
        public string highlightedOn = "HighlightedOn";
        public string pressedOn = "PressedOn";

        [Space(10)]
        public string normalOff = "NormalOff";
        public string highlightedOff = "HighlightedOff";
        public string pressedOff = "PressedOff";

        private Toggle toggle;

        private void Start ()
        {
            toggle = GetComponent<Toggle> ();
            toggle.onValueChanged.AddListener (OnToggleValueChanged);

            ResetTriggers ();
            toggle.animator.SetTrigger (toggle.isOn ? normalOn : normalOff);

            if ( hideOnToggle != null )
                hideOnToggle.SetActive (toggle.isOn);
        }
        
        private void OnToggleValueChanged (bool isOn)
        {
            var triggers = toggle.animationTriggers;
            if ( isOn )
            {
                triggers.normalTrigger = normalOn;
                triggers.highlightedTrigger = highlightedOn;
                triggers.pressedTrigger = pressedOn;
            }
            else
            {
                triggers.normalTrigger = normalOff;
                triggers.highlightedTrigger = highlightedOff;
                triggers.pressedTrigger = pressedOff;
            }
            toggle.animationTriggers = triggers;

            ResetTriggers ();

            toggle.animator.SetTrigger (isOn ? highlightedOn : highlightedOff);

            if ( hideOnToggle != null )
                hideOnToggle.SetActive (isOn);
        }
        private void ResetTriggers()
        {
            toggle.animator.ResetTrigger (normalOn);
            toggle.animator.ResetTrigger (highlightedOn);
            toggle.animator.ResetTrigger (pressedOn);

            toggle.animator.ResetTrigger (normalOff);
            toggle.animator.ResetTrigger (highlightedOff);
            toggle.animator.ResetTrigger (pressedOff);
        }
    }
}
