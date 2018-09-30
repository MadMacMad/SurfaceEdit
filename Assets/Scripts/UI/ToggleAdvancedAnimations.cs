using UnityEngine;
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
        public string highlightedOn = "NormalOn";
        public string pressedOn = "NormalOn";
        public string disabledOn = "NormalOn";

        [Space(10)]
        public string normalOff = "NormalOff";
        public string highlightedOff = "NormalOff";
        public string pressedOff = "NormalOff";
        public string disabledOff = "NormalOff";

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
                triggers.disabledTrigger = disabledOn;
            }
            else
            {
                triggers.normalTrigger = normalOff;
                triggers.highlightedTrigger = highlightedOff;
                triggers.pressedTrigger = pressedOff;
                triggers.disabledTrigger = disabledOff;
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
            toggle.animator.ResetTrigger (disabledOn);

            toggle.animator.ResetTrigger (normalOff);
            toggle.animator.ResetTrigger (highlightedOff);
            toggle.animator.ResetTrigger (pressedOff);
            toggle.animator.ResetTrigger (disabledOff);
        }
    }
}
