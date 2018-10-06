using System;

namespace SurfaceEdit
{
    public abstract class Singleton<T> where T : class
    {
        public static T Instance => instance.Value;
        private static Lazy<T> instance = new Lazy<T> (() => Activator.CreateInstance(typeof(T), true) as T);
    }
}
