using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilify
{
    public abstract class Singleton<T> where T : class, new()
    {
        public static T Instance => instance.Value;
        private static Lazy<T> instance = new Lazy<T> (() => new T ());
    }
}
