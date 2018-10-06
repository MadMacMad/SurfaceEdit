using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SurfaceEdit.Affectors;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit.Presenters
{
    public sealed class LayerStackPresenter
    {
        private LayerStack stack;

        private Dictionary<string, Toggle> layerGOs = new Dictionary<string, Toggle> ();
        private Layer activeLayer;

        private LayerStackViewData layerData;
        private ContextMenuViewData contextMenuData;

        public LayerStackPresenter (LayerStackViewData layerData, ContextMenuViewData contextMenuData, LayerStack stack)
        {
            Assert.NotNull (layerData, nameof (layerData));
            Assert.NotNull (contextMenuData, nameof (contextMenuData));
            Assert.NotNull (stack, nameof (stack));

            this.layerData = layerData;
            this.contextMenuData = contextMenuData;
            this.stack = stack;

            layerData.createLayerButton.onClick.AddListener (() => stack.CreateLayer());

            layerData.createAffectorButton.onClick.AddListener (() =>
            {
                if ( activeLayer != null )
                    AddAffector (activeLayer);
            });

            stack.OnLayerCreate += OnLayerCreate;
            stack.OnLayerDelete += OnLayerDelete;
        }

        public void SetActiveLayer (Layer layer)
        {
            Assert.ArgumentNotNull (layer, nameof (layer));
            Assert.ArgumentTrue (stack.Layers.Contains (layer), $"{nameof(LayerStack)} does not contains given layer!");

            activeLayer = layer;

            foreach ( var pair in layerGOs )
                pair.Value.isOn = false;

            layerGOs[layer.ID].isOn = true;
        }
        
        private void OnLayerCreate(Layer layer)
        {
            var go = GameObject.Instantiate (layerData.layerPrefab, layerData.layersParent);
            var toggle = go.GetComponent<Toggle> ();
            toggle.group = layerData.layersToggleGroup;

            go.GetComponent<MouseClickEventSender> ().OnRightClicked.AddListener(() =>
             {
                 var menu = new ContextMenuPresenter (contextMenuData, Input.mousePosition);
                 menu.AddMenuItem ("Delete", () => stack.DeleteLayer (layer));
                 menu.AddMenuItem ("Reset", () => layer.Reset());
                 menu.AddMenuItem ("Add Affector", () => AddAffector(layer));
             });

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

        private void AddAffector(Layer layer)
        {
            if ( layer == null )
                return;

            layer.AddAffector (new DummyAffector (stack.Context, stack.Context.Channels.Clone()));
        }
    }
}
