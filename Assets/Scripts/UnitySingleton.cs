using System;
using UnityEngine;

namespace Tilify
{
    public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance => instance.Value;
        private static Lazy<T> instance = new Lazy<T> (() => new GameObject(nameof(T)).AddComponent<T>());
    }
}
