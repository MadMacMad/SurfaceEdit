using System;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class UnitySingleton<Mono> : MonoBehaviour where Mono : MonoBehaviour
    {
        public static Mono Instance => instance.Value;
        private static Lazy<Mono> instance = new Lazy<Mono> (() =>
        {
            var gos = FindObjectsOfType<Mono> ();
            if ( gos.Length > 1 )
            {
                Debug.LogError ($"There is more than one({gos.Length}) GameObjects with {typeof (Mono).FullName} singleton script attached." +
                                $"All GameObjects, except one, will be destroyed.");
                for ( int i = 1; i < gos.Length; i++ )
                    Destroy (gos[i]);
                return gos[0];
            }
            return new GameObject (typeof (Mono).Name).AddComponent<Mono> ();
        });
    }
}
