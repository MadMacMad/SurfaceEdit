using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurfaceEdit.Presenters
{
    public sealed class ContextMenuPresenter : IDisposable
    {
        public event Action OnDispose;
        private bool isDisposed = false;

        private ContextMenuViewData data;

        private GameObject menu;
        
        public ContextMenuPresenter(ContextMenuViewData data, Vector3 position)
        {
            Assert.ArgumentNotNull (data, nameof (data));

            this.data = data;

            menu = GameObject.Instantiate (data.contextMenuPrefab);

            menu.GetComponent<ContextMenuDestroyer> ().OnDestroy += Dispose;

            var rectTransform = menu.GetComponent<RectTransform> ();
            rectTransform.SetParent(data.canvas.transform);
            rectTransform.position = position;
        }
        public void AddMenuItem(string text, Action callback)
        {
            Assert.True (!isDisposed, nameof (ContextMenuDestroyer) + " is already disposed!");
            Assert.ArgumentNotNull (callback, nameof (callback));
            Assert.ArgumentNotNullOrEmptry (text, nameof (text));

            var go = GameObject.Instantiate (data.contextMenuButtonPrefab, menu.transform);
            go.GetComponentInChildren<TextMeshProUGUI> ().text = text;

            go.GetComponent<Button> ().onClick.AddListener (() =>
            {
                callback ();
                GameObject.Destroy (menu);
                Dispose ();
            });
        }

        public void Dispose()
        {
            if ( isDisposed )
                return;
            GameObject.Destroy (menu);
            OnDispose?.Invoke ();
        }
    }
}
