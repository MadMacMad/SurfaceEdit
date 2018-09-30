using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SurfaceEdit
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteInEditMode]
    public class BindSliderValueToText : MonoBehaviour
    {
        public Slider slider;
        [Range(0, 100)]
        public float multiplier;
        public bool discardFraction;

        private TextMeshProUGUI text;

        private void OnEnable ()
        {
            text = GetComponent<TextMeshProUGUI> ();
            if ( slider != null )
            {
                slider.onValueChanged.AddListener (UpdateText);
                UpdateText (slider.value);
            }
        }
        private void UpdateText(float value)
        {
            if ( !discardFraction )
                text.text = ( value * multiplier ).ToString ();
            else
                text.text = Mathf.FloorToInt (value * multiplier).ToString ();
        }
    }
}