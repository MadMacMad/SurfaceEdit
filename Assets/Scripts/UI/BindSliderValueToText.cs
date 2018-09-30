using UnityEngine;
using System.Collections;
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
                slider.onValueChanged.AddListener (v =>
                {
                    if ( !discardFraction )
                        text.text = ( v * multiplier ).ToString ();
                    else
                        text.text = Mathf.FloorToInt (v * multiplier).ToString ();
                });
            }
        }
    }
}