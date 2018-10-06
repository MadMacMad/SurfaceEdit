using System;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class Resource : IDisposable
    {
        public ApplicationContext Context { get; private set; }
        public event Action Destroyed;
        public string DiskPathToResource { get; private set; }
        public string Name { get; private set; }
        public abstract Texture2D PreviewTexture { get; protected set; }

        public Resource (string name, string diskPathToResource, ApplicationContext context)
        {
            Assert.ArgumentNotNullOrEmptry (name, nameof (name));
            Assert.ArgumentNotNullOrEmptry (diskPathToResource, nameof (diskPathToResource));
            Assert.ArgumentNotNull (context, nameof (context));

            Name = name;
            DiskPathToResource = diskPathToResource;
            Context = context;
        }

        public void Dispose ()
        {
            Destroyed?.Invoke ();
            Dispose_Internal ();
            GameObject.DestroyImmediate (PreviewTexture);
        }

        protected virtual void Dispose_Internal () { }
    }
}
