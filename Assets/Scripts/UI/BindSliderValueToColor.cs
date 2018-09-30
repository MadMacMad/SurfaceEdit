using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SurfaceEdit
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class BindSliderValueToColor : MonoBehaviour
    {
        public Slider slider;
        public Gradient gradient;
        public Graphic targetGrahic;

        private TextMeshProUGUI text;

        private void OnEnable ()
        {
            text = GetComponent<TextMeshProUGUI> ();
            if ( slider != null )
            {
                slider.onValueChanged.AddListener (UpdateColors);
                UpdateColors (slider.value);
            }
        }
        private void UpdateColors(float value)
        {
            if ( targetGrahic != null )
            {
                var min = slider.minValue;
                var max = slider.maxValue;

                var time = Mathf.InverseLerp (min, max, value);
                targetGrahic.color = gradient.Evaluate (time);
            }
        }
    }
}