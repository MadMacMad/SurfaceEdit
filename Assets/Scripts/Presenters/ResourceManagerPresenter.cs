using System.Collections.Generic;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit.Presenters
{
    public sealed class ResourceManagerPresenter
    {
        private ResourceManagerViewData viewData;
        private ContextMenuViewData contextMenuViewData;
        private ResourceManager resourceManager;

        private List<(GameObject baseGO, GameObject imageGO, Resource resource)> gos = new List<(GameObject baseGO, GameObject imageGO, Resource resource)> ();

        public ResourceManagerPresenter(ResourceManagerViewData viewData, ContextMenuViewData contextMenuViewData, ResourceManager resourceManager)
        {
            Assert.ArgumentNotNull (viewData, nameof (viewData));
            Assert.ArgumentNotNull (contextMenuViewData, nameof (contextMenuViewData));
            Assert.ArgumentNotNull (resourceManager, nameof (resourceManager));

            this.viewData = viewData;
            this.contextMenuViewData = contextMenuViewData;
            this.resourceManager = resourceManager;

            foreach ( var resource in resourceManager.Resources )
                OnResourceAdded (resource);

            resourceManager.ResourceAdded += OnResourceAdded;
            resourceManager.ResourceDeleted += OnResourceDeleted;

            foreach(var button in viewData.importResourcesButtons)
            {
                button.onClick.AddListener (() =>
                 {
                     var paths = StandaloneFileBrowser.OpenFilePanel ("Import Resources", "", new ExtensionFilter[] { ResourceManager.SupportedExtensions }, true);
                     foreach ( var path in paths )
                     {
                         var result = resourceManager.TryImport (path, out _);
                         if ( !result.IsSuccessfull )
                             Debug.LogWarning (result.ErrorMessage);
                     }
                 });
            }
        }

        private void OnResourceAdded(Resource resource)
        {
            var baseGO = GameObject.Instantiate (viewData.resourcePrefab, viewData.resourcesParent.transform);
            var image = baseGO.GetComponentInChildren<Image> ();
            var imageGO = image.gameObject;

            baseGO.GetComponent<MouseClickEventSender> ().OnRightClicked.AddListener (() =>
              {
                  var menu = new ContextMenuPresenter (contextMenuViewData, Input.mousePosition);
                  menu.AddMenuItem ("Rename", () => Debug.Log ("TODO: Implement"));
                  menu.AddMenuItem ("Delete", () => resourceManager.DeleteResource (resource));
              });
            image.sprite = Sprite.Create (resource.PreviewTexture, new Rect (0, 0, resource.PreviewTexture.width, resource.PreviewTexture.height), Vector2.zero);

            gos.Add ((baseGO, imageGO, resource));
        }

        private void OnResourceDeleted(Resource resource)
        {
            var tuple = gos.Where (t => t.resource == resource).First();
            GameObject.Destroy (tuple.imageGO.GetComponent<Image> ().sprite);
            GameObject.Destroy (tuple.baseGO);

            gos.Remove (tuple);
        }
    }
}
