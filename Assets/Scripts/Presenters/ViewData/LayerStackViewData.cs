using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit
{
    public sealed class LayerStackViewData : MonoBehaviour
    {
        public GameObject layerControlPrefab;
        public Transform layersParent;
        public Button createLayerButton;
        public ToggleGroup layersToggleGroup;
    }
}
