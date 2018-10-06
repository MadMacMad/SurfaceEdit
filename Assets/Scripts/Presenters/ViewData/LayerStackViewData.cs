using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit.Presenters
{
    public sealed class LayerStackViewData : ViewData
    {
        public GameObject layerPrefab;
        public Transform layersParent;
        public Button createLayerButton;
        public Button createAffectorButton;
        public ToggleGroup layersToggleGroup;
    }
}
