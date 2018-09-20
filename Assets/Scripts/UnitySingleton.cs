using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance => instance.Value;
        private static Lazy<T> instance = new Lazy<T> (() => new GameObject(nameof(T)).AddComponent<T>());
    }
}
