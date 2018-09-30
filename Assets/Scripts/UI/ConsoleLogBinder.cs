using UnityEngine;
using TMPro;

namespace SurfaceEdit
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ConsoleLogBinder : MonoBehaviour
    {
        private TextMeshProUGUI text;

        private void Start ()
        {
            text = GetComponent<TextMeshProUGUI> ();
            Application.logMessageReceived += (logString, stackStace, logType) =>
            {
                var color = new Color32(200, 200, 200, 255);
                if ( logType == LogType.Error || logType == LogType.Exception )
                    color = new Color32(255, 0, 36, 255);
                else if ( logType == LogType.Warning )
                    color = new Color32 (255, 126, 0, 255);

                text.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(color).Substring(0, 6)}>{logString.Replace("\n", " ")}</color>";
            };
        }
    }
}