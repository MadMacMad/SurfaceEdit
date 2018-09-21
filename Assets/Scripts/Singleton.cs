using System;

namespace SurfaceEdit
{
    public abstract class Singleton<T> where T : class, new()
    {
        public static T Instance => instance.Value;
        private static Lazy<T> instance = new Lazy<T> (() => new T ());
    }
}
