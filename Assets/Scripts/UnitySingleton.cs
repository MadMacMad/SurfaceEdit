using System;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance => instance.Value;
        private static Lazy<T> instance = new Lazy<T> (() =>
        {
            var gos = GameObject.FindObjectsOfType<T> ();
            if ( gos.Length > 1 )
            {
                Debug.LogError ($"There is more then one({gos.Length}) GameObjects with {typeof (T).FullName} singleton script attached." +
                                $"All GameObjects expect one will be destroyed.");
                for ( int i = 1; i < gos.Length; i++ )
                    GameObject.Destroy (gos[i]);
                return gos[0];
            }
            return new GameObject (typeof (T).Name).AddComponent<T> ();
        });
    }
}
