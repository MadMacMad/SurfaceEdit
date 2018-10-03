using System;
using System.Collections.Generic;

namespace SurfaceEdit
{
    /// <summary>
    /// This class memorizes any value every frame on LateUpdate. Later in an Update method you can get this value and use it.
    /// This can be useful for calculating the difference in values between 2 frames.
    /// For example, the difference in mouse movement or something like that.
    /// </summary>
    public class UnityMemorizer<T> : Singleton<UnityMemorizer<T>>
    {
        private Dictionary<string, Func<T>> getters = new Dictionary<string, Func<T>> ();
        private Dictionary<string, T> values = new Dictionary<string, T> ();

        private UnityMemorizer()
        {
            UnityCallbackRegistrator.Instance.OnLateUpdate += LateUpdate;
        }

        public void Memorize(string name, Func<T> getter)
        {
            Assert.ArgumentTrue (!string.IsNullOrEmpty (name), nameof (name) + " is null of empty!");
            Assert.ArgumentNotNull (getter, nameof (getter));
            Assert.ArgumentTrue (!getters.ContainsKey (name), $"There is already a memorized value with \"{name}\" name!");

            getters.Add (name, getter);
            values.Add (name, getter());
        }
        public T GetValue(string name)
        {
            Assert.ArgumentTrue (!string.IsNullOrEmpty (name), nameof (name) + " is null of empty!");
            Assert.ArgumentTrue (getters.ContainsKey (name), "The Memorized value named " + nameof (name) + " does not exist!");

            return values[name];
        }

        private void LateUpdate()
        {
            foreach ( var pair in getters )
                values[pair.Key] = pair.Value ();
        }
    }
}
