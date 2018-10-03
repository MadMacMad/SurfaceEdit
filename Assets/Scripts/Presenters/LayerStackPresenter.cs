using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit
{
    public sealed class LayerStackPresenter
    {
        private LayerStack stack;

        private Dictionary<string, Toggle> layerGOs = new Dictionary<string, Toggle> ();
        private Layer activeLayer;

        private LayerStackViewData data;

        public LayerStackPresenter (LayerStackViewData data, LayerStack stack)
        {
            Assert.NotNull (data, nameof (data));
            Assert.NotNull (stack, nameof (stack));

            this.data = data;
            this.stack = stack;

            data.createLayerButton.onClick.AddListener (() => stack.CreateLayer());
            stack.OnLayerCreate += OnLayerCreate;
            stack.OnLayerDelete += OnLayerDelete;
        }

        public void SetActiveLayer (Layer layer)
        {
            Assert.ArgumentNotNull (layer, nameof (layer));
            Assert.ArgumentTrue (stack.Layers.Contains (layer), $"{nameof(LayerStack)} does not contains given layer!");

            activeLayer = layer;

            layerGOs[layer.ID].isOn = true;
        }
        
        private void OnLayerCreate(Layer layer)
        {
            var go = GameObject.Instantiate (data.layerControlPrefab, data.layersParent);
            var toggle = go.GetComponent<Toggle> ();
            toggle.group = data.layersToggleGroup;

            var deleteButton = go.GetComponentsInChildren<Button> ().Where (b => b.gameObject.name == "Delete").First ();
            deleteButton.onClick.AddListener (() => stack.DeleteLayer(layer));

            layerGOs.Add (layer.ID, toggle);
            SetActiveLayer (layer);
        }
        private void OnLayerDelete(Layer layer)
        {
            GameObject.Destroy (layerGOs[layer.ID].gameObject);
            layerGOs.Remove (layer.ID);

            if ( layerGOs.Count > 0 )
                layerGOs.First ().Value.isOn = true;
        }
    }
}
