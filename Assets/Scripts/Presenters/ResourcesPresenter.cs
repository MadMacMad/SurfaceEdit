using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFB;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit.Presenters
{
    public sealed class ResourcesPresenter
    {
        private ResourcesViewData viewData;
        private ContextMenuViewData contextMenuViewData;

        private List<(GameObject baseGO, GameObject imageGO, SResource resource)> gos = new List<(GameObject baseGO, GameObject imageGO, SResource resource)> ();

        public ResourcesPresenter(ResourcesViewData viewData, ContextMenuViewData contextMenuViewData)
        {
            Assert.ArgumentNotNull (viewData, nameof (viewData));
            Assert.ArgumentNotNull (contextMenuViewData, nameof (contextMenuViewData));

            this.viewData = viewData;
            this.contextMenuViewData = contextMenuViewData;

            foreach ( var resource in SResources.Instance.Resources )
                OnResourceAdded (resource);

            SResources.Instance.ResourceAdded += OnResourceAdded;
            SResources.Instance.ResourceDeleted += OnResourceDeleted;

            foreach(var button in viewData.importResourcesButtons)
            {
                button.onClick.AddListener (() =>
                 {
                     var paths = StandaloneFileBrowser.OpenFilePanel ("Import Resources", "", new ExtensionFilter[] { SResources.SupportedExtensions }, true);
                     foreach ( var path in paths )
                         SResources.Instance.TryImport (path, out _);
                 });
            }
        }

        private void OnResourceAdded(SResource resource)
        {
            var baseGO = GameObject.Instantiate (viewData.resourcePrefab, viewData.resourcesParent.transform);
            var image = baseGO.GetComponentInChildren<Image> ();
            var imageGO = image.gameObject;

            baseGO.GetComponent<MouseClickEventSender> ().OnRightClicked.AddListener (() =>
              {
                  var menu = new ContextMenuPresenter (contextMenuViewData, Input.mousePosition);
                  menu.AddMenuItem ("Rename", () => Debug.Log ("TODO: Implement"));
                  menu.AddMenuItem ("Delete", () => SResources.Instance.DeleteResource (resource));
              });
            image.sprite = Sprite.Create (resource.PreviewTexture, new Rect (0, 0, resource.PreviewTexture.width, resource.PreviewTexture.height), Vector2.zero);

            gos.Add ((baseGO, imageGO, resource));
        }

        private void OnResourceDeleted(SResource resource)
        {
            var tuple = gos.Where (t => t.resource == resource).First();
            GameObject.Destroy (tuple.imageGO.GetComponent<Image> ().sprite);
            GameObject.Destroy (tuple.baseGO);

            gos.Remove (tuple);
        }
    }
}
