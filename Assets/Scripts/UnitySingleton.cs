using System;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance => instance.Value;
        private static Lazy<T> instance = new Lazy<T> (() =>
        {
            var gos = FindObjectsOfType<T> ();
            if ( gos.Length > 1 )
            {
                Debug.LogError ($"There is more than one({gos.Length}) GameObjects with {typeof (T).FullName} singleton script attached." +
                                $"All GameObjects, except one, will be destroyed.");
                for ( int i = 1; i < gos.Length; i++ )
                    Destroy (gos[i]);
                return gos[0];
            }
            return new GameObject (typeof (T).Name).AddComponent<T> ();
        });
    }
}
