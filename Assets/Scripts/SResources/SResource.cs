using System;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class SResource : IDisposable
    {
        public string Name { get; private set; }
        public abstract Texture2D PreviewTexture { get; protected set; }

        public SResource (string name)
        {
            Assert.ArgumentNotNullOrEmptry (name, nameof (name));
            Name = name;
        }

        public abstract void Dispose ();
    }
}
