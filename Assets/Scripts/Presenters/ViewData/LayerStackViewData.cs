using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
