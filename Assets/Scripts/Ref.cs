using System;

namespace Tilify
{
    public sealed class Ref<T>
    {
        public T Value { get => getter (); set => setter (value); }

        private readonly Action<T> setter;
        private readonly Func<T> getter;

        public Ref (Action<T> setter, Func<T> getter)
        {
            this.setter = setter;
            this.getter = getter;
        }
    }
}
